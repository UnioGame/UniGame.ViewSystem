#if UNITY_6000_5_OR_NEWER
namespace UniGame.UiSystem.Runtime.UiToolkit.PlayMode.Tests
{
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using NUnit.Framework;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.EventSystems;
    using UnityEngine.TestTools;
    using UnityEngine.UI;
    using UnityEngine.UIElements;
    using UniGame.Core.Runtime;
    using UniGame.Runtime.DataFlow;
    using UniGame.Runtime.ObjectPool;
    using UniGame.UiSystem.Runtime;

    public sealed class UiToolkitViewSystemPlayModeTests
    {
        internal const string ValidPrefabGuid = "d26de4d05349c7f45aa86728cadb8a34";
        private const string VisualTreePath = "Assets/Game.Content/Ui/UIToolkitSandbox/UXML/UIToolkitSmokeTestPopup.uxml";

        [UnityTest]
        public IEnumerator ViewRebindsModelTreeAfterDisableAndReleasesBindingsOnClose() =>
            RebindsAfterDisable().ToCoroutine();

        [UnityTest]
        public IEnumerator ComponentDisableRestoresInputWhenPanelRendererStaysActive() =>
            RestoresInputAfterComponentDisable().ToCoroutine();

        [UnityTest]
        public IEnumerator LayoutOrdersMixedViewsAndRestoresTheirOriginalSettingsOnRemoval() =>
            LayoutsMixedStack().ToCoroutine();

        [UnityTest]
        public IEnumerator ViewFactoryCreatesPanelRendererPrefabAndSupportsPoolReuse() =>
            CreatesAndReusesPrefab().ToCoroutine();

        private static async UniTask RebindsAfterDisable()
        {
            var gameObject = CreateToolkitView("UITK lifecycle test", out var view, out var panelSettings);
            try
            {
                var bound = 0;
                var unbound = 0;
                view.OnBind = () =>
                {
                    bound++;
                    view.AddUnbind(() => unbound++);
                };

                await view.Initialize(new EmptyViewModel());
                Assert.That(bound, Is.EqualTo(1));

                gameObject.SetActive(false);
                Assert.That(unbound, Is.EqualTo(1));
                gameObject.SetActive(true);
                Assert.That(bound, Is.EqualTo(2));

                var adapter = gameObject.AddComponent<UiToolkitMixedOrderAdapter>();
                adapter.Apply(1, true);
                for (var frame = 0; frame < 120 && bound < 3; frame++)
                    await UniTask.Yield();
                Assert.That(bound, Is.EqualTo(3));
                Assert.That(unbound, Is.EqualTo(2));

                adapter.Release();
                for (var frame = 0; frame < 120 && bound < 4; frame++)
                    await UniTask.Yield();
                Assert.That(bound, Is.EqualTo(4));
                Assert.That(unbound, Is.EqualTo(3));

                view.Destroy();
                Assert.That(unbound, Is.EqualTo(4));
            }
            finally
            {
                Object.Destroy(gameObject);
                Object.Destroy(panelSettings);
            }
        }

        private static async UniTask RestoresInputAfterComponentDisable()
        {
            var gameObject = CreateToolkitView("UITK component toggle test", out var view, out var panelSettings);
            try
            {
                var bindings = 0;
                view.OnBind = () => bindings++;
                await view.Initialize(new EmptyViewModel());
                view.SetVisible(true);
                var root = view.Root;
                var originalMode = root.pickingMode;
                Assert.That(originalMode, Is.Not.EqualTo(PickingMode.Ignore));

                view.enabled = false;
                Assert.That(view.IsToolkitReady, Is.False);
                Assert.That(gameObject.GetComponent<PanelRenderer>().enabled, Is.True);
                Assert.That(root.pickingMode, Is.EqualTo(PickingMode.Ignore));
                view.enabled = true;
                for (var frame = 0; frame < 120 && bindings < 2; frame++)
                    await UniTask.Yield();

                Assert.That(bindings, Is.EqualTo(2));
                Assert.That(view.Root.pickingMode, Is.EqualTo(originalMode));
                view.Destroy();
            }
            finally
            {
                Object.Destroy(gameObject);
                Object.Destroy(panelSettings);
            }
        }

        private static async UniTask LayoutsMixedStack()
        {
            var toolkitObject = CreateToolkitView("UITK stack view", out var toolkitView, out var panelSettings);
            var canvasObject = new GameObject("uGUI stack view", typeof(RectTransform));
            var canvasView = canvasObject.AddComponent<UiToolkitStackPlayModeTestView>();
            canvasView.viewAnimation = new UniGame.ViewSystem.Runtime.Animations.SimpleFadeViewAnimation();
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.overrideSorting = false;
            canvas.sortingOrder = 17;
            var button = canvasObject.AddComponent<UnityEngine.UI.Button>();
            var canvasGroup = canvasObject.AddComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            var eventSystemObject = new GameObject("UITK stack EventSystem");
            var eventSystem = eventSystemObject.AddComponent<EventSystem>();
            var sourcePanel = toolkitView.PanelRenderer.panelSettings;
            var layout = new ViewLayout();

            try
            {
                layout.Push(canvasView);
                await canvasView.ShowAsync();
                eventSystem.SetSelectedGameObject(button.gameObject);
                layout.Push(toolkitView);
                await toolkitView.ShowAsync();

                var toolkitOrder = toolkitObject.GetComponent<UiToolkitMixedOrderAdapter>();
                var canvasOrder = canvasObject.GetComponent<UiToolkitMixedOrderAdapter>();
                Assert.That(toolkitOrder, Is.Not.Null);
                Assert.That(canvasOrder, Is.Not.Null);
                Assert.That(canvasOrder.CurrentStackIndex, Is.EqualTo(0));
                Assert.That(toolkitOrder.CurrentStackIndex, Is.EqualTo(1));
                Assert.That(canvas.overrideSorting, Is.True);
                Assert.That(canvas.sortingOrder, Is.EqualTo(0));
                Assert.That(canvasGroup.interactable, Is.False);
                Assert.That(canvasGroup.blocksRaycasts, Is.False);
                Assert.That(toolkitView.PanelRenderer.panelSettings, Is.Not.SameAs(sourcePanel));
                Assert.That(toolkitView.PanelRenderer.panelSettings.sortingOrder, Is.EqualTo(10));
                Assert.That(eventSystem.currentSelectedGameObject, Is.Null);

                layout.Close(toolkitView);

                Assert.That(toolkitOrder.CurrentStackIndex, Is.EqualTo(-1));
                Assert.That(canvas.sortingOrder, Is.EqualTo(0));
                Assert.That(canvasGroup.interactable, Is.True);
                Assert.That(canvasGroup.blocksRaycasts, Is.True);
                Assert.That(eventSystem.currentSelectedGameObject, Is.SameAs(button.gameObject));
                Assert.That(toolkitView.PanelRenderer.panelSettings, Is.SameAs(sourcePanel));
                Assert.That(canvasOrder.CurrentStackIndex, Is.EqualTo(0));

                layout.Dispose();

                Assert.That(toolkitOrder.CurrentStackIndex, Is.EqualTo(-1));
                Assert.That(canvasOrder.CurrentStackIndex, Is.EqualTo(-1));
                Assert.That(canvas.overrideSorting, Is.False);
                Assert.That(canvas.sortingOrder, Is.EqualTo(17));
                Assert.That(canvasGroup.interactable, Is.False);
                Assert.That(canvasGroup.blocksRaycasts, Is.False);
                Assert.That(toolkitView.PanelRenderer.panelSettings, Is.SameAs(sourcePanel));
                Assert.That(toolkitObject.GetComponent<UiToolkitMixedOrderAdapter>().CurrentStackIndex, Is.EqualTo(-1));
            }
            finally
            {
                layout.Dispose();
                Object.Destroy(toolkitObject);
                Object.Destroy(canvasObject);
                Object.Destroy(eventSystemObject);
                Object.Destroy(panelSettings);
            }
        }

        private static async UniTask CreatesAndReusesPrefab()
        {
            var source = CreateToolkitView("UITK factory source", out _, out var panelSettings);
            var sourcePanel = source.GetComponent<PanelRenderer>().panelSettings;
            var lifetime = new LifeTime();
            var factory = new UiToolkitViewFactoryPlayModeTestAdapter(source);
            GameObject created = null;

            try
            {
                var result = await factory.CreateForTest(lifetime, true);
                Assert.That(result.View, Is.Not.Null);
                created = result.View.GameObject;
                Assert.That(created, Is.Not.SameAs(source));
                Assert.That(created.GetComponent<PanelRenderer>().panelSettings, Is.SameAs(sourcePanel));

                ObjectPool.Despawn(created);
                var reused = await factory.CreateForTest(lifetime, true);
                Assert.That(reused.View.GameObject, Is.SameAs(created));
                Assert.That(reused.View.GameObject.GetComponent<PanelRenderer>().panelSettings, Is.SameAs(sourcePanel));
            }
            finally
            {
                lifetime.Terminate();
                factory.Dispose();
                ObjectPool.DestroyPool(source);
                Object.Destroy(source);
                Object.Destroy(panelSettings);
            }
        }

        private static GameObject CreateToolkitView(
            string name,
            out UiToolkitStackPlayModeTestView view,
            out PanelSettings panelSettings)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.SetActive(false);
            var renderer = gameObject.AddComponent<PanelRenderer>();
            panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            renderer.panelSettings = panelSettings;
            renderer.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreePath);
            Assert.That(renderer.visualTreeAsset, Is.Not.Null, $"Missing UITK test VisualTreeAsset at '{VisualTreePath}'.");
            view = gameObject.AddComponent<UiToolkitStackPlayModeTestView>();
            view.viewAnimation = new UniGame.ViewSystem.Runtime.Animations.SimpleFadeViewAnimation();
            gameObject.SetActive(true);
            return gameObject;
        }
    }

    public sealed class UiToolkitStackPlayModeTestView : UiToolkitView<EmptyViewModel>
    {
        public System.Action OnBind;

        protected override UniTask OnInitializeToolkit(EmptyViewModel model, VisualElement root)
        {
            OnBind?.Invoke();
            return UniTask.CompletedTask;
        }
    }

    public sealed class UiToolkitViewFactoryPlayModeTestAdapter : ViewFactory
    {
        private readonly GameObject _source;

        public UiToolkitViewFactoryPlayModeTestAdapter(GameObject source) : base(null, null)
        {
            _source = source;
        }

        public UniTask<ViewResult> CreateForTest(ILifeTime lifetime, bool usePooling)
        {
            var asset = new AssetReferenceGameObject(UiToolkitViewSystemPlayModeTests.ValidPrefabGuid);
            return Create(asset, lifetime, usePooling: usePooling, activateOnCreate: false);
        }

        protected override UniTask<GameObject> LoadAssetReferenceAsset(AssetReferenceGameObject asset, ILifeTime lifeTime)
        {
            return UniTask.FromResult(_source);
        }
    }
}
#else
namespace UniGame.UiSystem.Runtime.UiToolkit.PlayMode.Tests
{
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using NUnit.Framework;
    using UniGame.UiSystem.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniGame.UiSystem.Runtime;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEngine.UIElements;

    public sealed class UiToolkitDocumentTests
    {
        private const string VisualTreePath = "Assets/Game.Content/Ui/WorldspaceUIToolkit/BarkView.uxml";
        private const string PanelSettingsPath = "Assets/Game.Content/Ui/_Settings/UIDefaultPanelSettings-Worldspace.asset";

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator UiDocumentRebindsAndRestoresPanelSettings() => VerifyLifecycle().ToCoroutine();

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator NestedUiDocumentFollowsParentPanel() => VerifyNestedDocument().ToCoroutine();

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator DisabledViewDoesNotLeaveUiDocumentInteractive() => VerifyViewDisable().ToCoroutine();

        private static async UniTask VerifyLifecycle()
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreePath);
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            Assert.That(tree, Is.Not.Null);
            Assert.That(panelSettings, Is.Not.Null);

            var gameObject = new GameObject("UITK UIDocument view test");
            gameObject.SetActive(false);
            var document = gameObject.AddComponent<UIDocument>();
            document.visualTreeAsset = tree;
            document.panelSettings = panelSettings;
            var view = gameObject.AddComponent<UiToolkitDocumentTestView>();
            var layoutObject = new GameObject("UITK test layout", typeof(RectTransform));
            var layoutCanvas = layoutObject.AddComponent<Canvas>();
            layoutCanvas.sortingOrder = 125;
            try
            {
                gameObject.SetActive(true);
                await view.Initialize(new EmptyViewModel());
                Assert.That(view.Root, Is.SameAs(document.rootVisualElement));
                Assert.That(view.BindCount, Is.EqualTo(1));
                Assert.That(view.UnbindCount, Is.Zero);
                view.SetVisible(true);
                Assert.That(view.Root.pickingMode, Is.EqualTo(PickingMode.Position));

                var adapter = gameObject.AddComponent<UiToolkitMixedOrderAdapter>();
                adapter.Apply(0, false, ViewType.Screen, layoutCanvas.sortingOrder);
                await UniTask.WaitUntil(() => view.Root == document.rootVisualElement && view.BindCount >= 2);
                Assert.That(document.panelSettings, Is.Not.SameAs(panelSettings));
                Assert.That(document.sortingOrder, Is.EqualTo(125f));
                Assert.That(view.Root.pickingMode, Is.EqualTo(PickingMode.Ignore));
                Assert.That(view.UnbindCount, Is.GreaterThanOrEqualTo(1));
                adapter.Release();
                await UniTask.WaitUntil(() => view.Root == document.rootVisualElement && view.BindCount >= 3);
                Assert.That(document.panelSettings, Is.SameAs(panelSettings));
                Assert.That(view.Root.pickingMode, Is.EqualTo(PickingMode.Position));
                Assert.That(view.UnbindCount, Is.GreaterThanOrEqualTo(2));

                document.enabled = false;
                await UniTask.WaitUntil(() => view.Root == null);
                Assert.That(view.UnbindCount, Is.GreaterThanOrEqualTo(3));
                document.enabled = true;
                await UniTask.WaitUntil(() => view.Root != null && view.BindCount >= 4);
                Assert.That(view.Root, Is.SameAs(document.rootVisualElement));
            }
            finally
            {
                view.Destroy();
                Object.DestroyImmediate(gameObject);
                Object.DestroyImmediate(layoutObject);
            }
        }

        private static async UniTask VerifyNestedDocument()
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreePath);
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            Assert.That(tree, Is.Not.Null);
            Assert.That(panelSettings, Is.Not.Null);

            var parentObject = new GameObject("UITK parent document test");
            parentObject.SetActive(false);
            var childObject = new GameObject("UITK nested document test");
            childObject.transform.SetParent(parentObject.transform, false);
            var parentDocument = parentObject.AddComponent<UIDocument>();
            parentDocument.visualTreeAsset = tree;
            parentDocument.panelSettings = panelSettings;
            var childDocument = childObject.AddComponent<UIDocument>();
            childDocument.visualTreeAsset = tree;
            var parentView = parentObject.AddComponent<UiToolkitDocumentTestView>();
            var childView = childObject.AddComponent<UiToolkitDocumentTestView>();
            var childSourcePanel = childDocument.panelSettings;
            var layoutObject = new GameObject("UITK nested test layout", typeof(RectTransform));
            var layoutCanvas = layoutObject.AddComponent<Canvas>();
            layoutCanvas.sortingOrder = 125;
            try
            {
                parentObject.SetActive(true);
                await UniTask.WaitUntil(() => childDocument.parentUI == parentDocument);
                await parentView.Initialize(new EmptyViewModel());
                await childView.Initialize(new EmptyViewModel());
                Assert.That(childView.Root, Is.SameAs(childDocument.rootVisualElement));
                Assert.That(childView.BindCount, Is.EqualTo(1));

                var childAdapter = childObject.AddComponent<UiToolkitMixedOrderAdapter>();
                childAdapter.Apply(0, true, ViewType.Screen, layoutCanvas.sortingOrder);
                Assert.That(childDocument.panelSettings, Is.SameAs(childSourcePanel));

                var adapter = parentObject.AddComponent<UiToolkitMixedOrderAdapter>();
                adapter.Apply(0, true, ViewType.Screen, layoutCanvas.sortingOrder);
                await UniTask.WaitUntil(() => parentView.BindCount >= 2 && childView.BindCount >= 2);
                Assert.That(childDocument.panelSettings, Is.SameAs(parentDocument.panelSettings));
                Assert.That(childView.Root, Is.SameAs(childDocument.rootVisualElement));

                adapter.Release();
                await UniTask.WaitUntil(() => parentView.BindCount >= 3 && childView.BindCount >= 3);
                Assert.That(parentDocument.panelSettings, Is.SameAs(panelSettings));
                Assert.That(childDocument.panelSettings, Is.SameAs(childSourcePanel));
                childAdapter.Release();
            }
            finally
            {
                childView.Destroy();
                parentView.Destroy();
                Object.DestroyImmediate(parentObject);
                Object.DestroyImmediate(layoutObject);
            }
        }

        private static async UniTask VerifyViewDisable()
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreePath);
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            Assert.That(tree, Is.Not.Null);
            Assert.That(panelSettings, Is.Not.Null);

            var gameObject = new GameObject("UITK UIDocument component toggle test");
            gameObject.SetActive(false);
            var document = gameObject.AddComponent<UIDocument>();
            document.visualTreeAsset = tree;
            document.panelSettings = panelSettings;
            var view = gameObject.AddComponent<UiToolkitDocumentTestView>();
            try
            {
                gameObject.SetActive(true);
                await view.Initialize(new EmptyViewModel());
                view.SetVisible(true);
                var root = view.Root;
                Assert.That(root.pickingMode, Is.EqualTo(PickingMode.Position));

                view.enabled = false;
                Assert.That(document.enabled, Is.True);
                Assert.That(root.pickingMode, Is.EqualTo(PickingMode.Ignore));
                Assert.That(view.IsToolkitReady, Is.False);

                view.enabled = true;
                await UniTask.WaitUntil(() => view.BindCount >= 2);
                Assert.That(view.Root.pickingMode, Is.EqualTo(PickingMode.Ignore));
                view.SetVisible(true);
                Assert.That(view.Root.pickingMode, Is.EqualTo(PickingMode.Position));
            }
            finally
            {
                view.Destroy();
                Object.DestroyImmediate(gameObject);
            }
        }
    }

    public sealed class UiToolkitDocumentTestView : UiToolkitView<EmptyViewModel>
    {
        public int BindCount;
        public int UnbindCount;

        protected override UniTask OnInitializeToolkit(EmptyViewModel model, VisualElement root)
        {
            BindCount++;
            AddUnbind(() => UnbindCount++);
            return UniTask.CompletedTask;
        }
    }
}
#endif
