namespace UniGame.UiSystem.Runtime.UiToolkit.PlayMode.Tests
{
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using NUnit.Framework;
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

        [UnityTest]
        public IEnumerator ViewRebindsModelTreeAfterDisableAndReleasesBindingsOnClose() =>
            RebindsAfterDisable().ToCoroutine();

        [UnityTest]
        public IEnumerator LayoutOrdersMixedViewsAndRestoresTheirOriginalSettingsOnRemoval() =>
            LayoutsMixedStack().ToCoroutine();

        [UnityTest]
        public IEnumerator ViewFactoryCreatesUidocumentPrefabAndSupportsPoolReuse() =>
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
                    view.Presentation.AddUnbind(() => unbound++);
                };

                await view.Initialize(new EmptyViewModel());
                Assert.That(bound, Is.EqualTo(1));

                gameObject.SetActive(false);
                Assert.That(unbound, Is.EqualTo(1));
                gameObject.SetActive(true);
                Assert.That(bound, Is.EqualTo(2));

                view.Destroy();
                Assert.That(unbound, Is.EqualTo(2));
            }
            finally
            {
                Object.Destroy(gameObject.GetComponent<UIDocument>().visualTreeAsset);
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
            var sourcePanel = toolkitView.Presentation.Document.panelSettings;
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
                Assert.That(toolkitView.Presentation.Document.panelSettings, Is.Not.SameAs(sourcePanel));
                Assert.That(toolkitView.Presentation.Document.panelSettings.sortingOrder, Is.EqualTo(10));
                Assert.That(eventSystem.currentSelectedGameObject, Is.Null);

                layout.Close(toolkitView);

                Assert.That(toolkitOrder.CurrentStackIndex, Is.EqualTo(-1));
                Assert.That(canvas.sortingOrder, Is.EqualTo(0));
                Assert.That(canvasGroup.interactable, Is.True);
                Assert.That(canvasGroup.blocksRaycasts, Is.True);
                Assert.That(eventSystem.currentSelectedGameObject, Is.SameAs(button.gameObject));
                Assert.That(toolkitView.Presentation.Document.panelSettings, Is.SameAs(sourcePanel));
                Assert.That(canvasOrder.CurrentStackIndex, Is.EqualTo(0));

                layout.Dispose();

                Assert.That(toolkitOrder.CurrentStackIndex, Is.EqualTo(-1));
                Assert.That(canvasOrder.CurrentStackIndex, Is.EqualTo(-1));
                Assert.That(canvas.overrideSorting, Is.False);
                Assert.That(canvas.sortingOrder, Is.EqualTo(17));
                Assert.That(canvasGroup.interactable, Is.False);
                Assert.That(canvasGroup.blocksRaycasts, Is.False);
                Assert.That(toolkitView.Presentation.Document.panelSettings, Is.SameAs(sourcePanel));
                Assert.That(toolkitObject.GetComponent<UiToolkitMixedOrderAdapter>().CurrentStackIndex, Is.EqualTo(-1));
            }
            finally
            {
                layout.Dispose();
                Object.Destroy(toolkitView.Presentation.Document.visualTreeAsset);
                Object.Destroy(toolkitObject);
                Object.Destroy(canvasObject);
                Object.Destroy(eventSystemObject);
                Object.Destroy(panelSettings);
            }
        }

        private static async UniTask CreatesAndReusesPrefab()
        {
            var source = CreateToolkitView("UITK factory source", out _, out var panelSettings);
            var sourcePanel = source.GetComponent<UIDocument>().panelSettings;
            var lifetime = new LifeTime();
            var factory = new UiToolkitViewFactoryPlayModeTestAdapter(source);
            GameObject created = null;

            try
            {
                var result = await factory.CreateForTest(lifetime, true);
                Assert.That(result.View, Is.Not.Null);
                created = result.View.GameObject;
                Assert.That(created, Is.Not.SameAs(source));
                Assert.That(created.GetComponent<UIDocument>().panelSettings, Is.SameAs(sourcePanel));

                ObjectPool.Despawn(created);
                var reused = await factory.CreateForTest(lifetime, true);
                Assert.That(reused.View.GameObject, Is.SameAs(created));
                Assert.That(reused.View.GameObject.GetComponent<UIDocument>().panelSettings, Is.SameAs(sourcePanel));
            }
            finally
            {
                lifetime.Terminate();
                factory.Dispose();
                ObjectPool.DestroyPool(source);
                Object.Destroy(source.GetComponent<UIDocument>().visualTreeAsset);
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
            var document = gameObject.AddComponent<UIDocument>();
            panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            document.panelSettings = panelSettings;
            document.visualTreeAsset = ScriptableObject.CreateInstance<VisualTreeAsset>();
            gameObject.AddComponent<UiToolkitViewPresentation>();
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
