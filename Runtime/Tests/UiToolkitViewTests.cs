#if UNITY_6000_5_OR_NEWER
namespace UniGame.UiSystem.Runtime.UiToolkit.Tests
{
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using NUnit.Framework;
    using UniGame.UiSystem.Runtime;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEngine.UIElements;
    using UniModules.UniGame.UiSystem.Runtime;

    public sealed class UiToolkitViewTests
    {
        private const string VisualTreePath = "Assets/Game.Content/Ui/WorldspaceUIToolkit/BarkView.uxml";
        private const string PanelSettingsPath = "Assets/Game.Content/Ui/_Settings/UIDefaultPanelSettings-Worldspace.asset";

        [Test]
        public void BindingActionsAreReleasedExactlyOnce()
        {
            var gameObject = new GameObject("UITK binding lifetime test");
            try
            {
                var view = gameObject.AddComponent<UiToolkitEditModeTestView>();
                var released = 0;
                for (var cycle = 0; cycle < 20; cycle++)
                {
                    view.AddUnbind(() => released++);
                    view.ClearBindings();
                    view.ClearBindings();
                    Assert.That(released, Is.EqualTo(cycle + 1));
                }
            }
            finally { Object.DestroyImmediate(gameObject); }
        }

        [Test]
        public void MixedOrderAdapterKeepsBuiltInLayersInSeparateRangesAndRestoresSources()
        {
            var sourcePanel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            Assert.That(sourcePanel, Is.Not.Null);

            var toolkitObject = new GameObject("Layer order PanelRenderer test", typeof(RectTransform));
            var canvasObject = new GameObject("Layer order Canvas test", typeof(RectTransform));
            var renderer = toolkitObject.AddComponent<PanelRenderer>();
            renderer.panelSettings = sourcePanel;
            renderer.sortingOrder = 7;
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.sortingOrder = 17;
            var toolkitAdapter = toolkitObject.AddComponent<UiToolkitMixedOrderAdapter>();
            var canvasAdapter = canvasObject.AddComponent<UiToolkitMixedOrderAdapter>();

            try
            {
                var layers = new[] { ViewType.Screen, ViewType.Window, ViewType.Overlay };
                var bases = new[] { 125, 350, 575 };
                for (var index = 0; index < layers.Length; index++)
                {
                    toolkitAdapter.Apply(0, true, layers[index], bases[index]);
                    canvasAdapter.Apply(99, true, layers[index], bases[index]);
                    Assert.That(renderer.panelSettings.sortingOrder, Is.EqualTo(bases[index]));
                    Assert.That(renderer.sortingOrder, Is.EqualTo(bases[index]));
                    Assert.That(canvas.sortingOrder, Is.EqualTo(bases[index] + 99));
                }

                Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                    toolkitAdapter.Apply(100, true, ViewType.Screen, bases[0]));
                Assert.That(renderer.sortingOrder, Is.EqualTo(575));

                toolkitAdapter.Apply(1, true, ViewType.Window, bases[1]);
                toolkitAdapter.SetRequestedOrder(10);
                Assert.That(renderer.panelSettings.sortingOrder, Is.EqualTo(360));
                Assert.That(renderer.sortingOrder, Is.EqualTo(360));
                toolkitAdapter.Apply(2, true, ViewType.Window, bases[1]);
                Assert.That(renderer.panelSettings.sortingOrder, Is.EqualTo(360));
                toolkitAdapter.SetRequestedOrder(0);
                Assert.That(renderer.panelSettings.sortingOrder, Is.EqualTo(350));
                Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                    toolkitAdapter.SetRequestedOrder(100));
                Assert.That(renderer.panelSettings.sortingOrder, Is.EqualTo(350));
                toolkitAdapter.ClearRequestedOrder();
                Assert.That(renderer.panelSettings.sortingOrder, Is.EqualTo(352));
                Assert.That(renderer.sortingOrder, Is.EqualTo(352));
                Assert.That(toolkitAdapter.CurrentStackIndex, Is.EqualTo(2));

                toolkitAdapter.Apply(2, true, ViewType.Window, 400);
                Assert.That(renderer.panelSettings.sortingOrder, Is.EqualTo(402));
                Assert.That(renderer.sortingOrder, Is.EqualTo(402));

                toolkitAdapter.Release();
                canvasAdapter.Release();
                Assert.That(renderer.panelSettings, Is.SameAs(sourcePanel));
                Assert.That(renderer.sortingOrder, Is.EqualTo(7));
                Assert.That(canvas.sortingOrder, Is.EqualTo(17));
                Assert.That(canvas.overrideSorting, Is.False);

                Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                    toolkitAdapter.SetRequestedOrder(100000));
                toolkitAdapter.SetRequestedOrder(12);
                Assert.That(renderer.panelSettings.sortingOrder, Is.EqualTo(12));
                Assert.That(renderer.sortingOrder, Is.EqualTo(12));
                toolkitAdapter.ClearRequestedOrder();
                Assert.That(renderer.panelSettings, Is.SameAs(sourcePanel));
                Assert.That(renderer.sortingOrder, Is.EqualTo(7));
                toolkitAdapter.Apply(2, true);
                Assert.That(renderer.panelSettings.sortingOrder, Is.EqualTo(20));
                Assert.That(renderer.sortingOrder, Is.EqualTo(7));
            }
            finally
            {
                toolkitAdapter.Release();
                canvasAdapter.Release();
                Object.DestroyImmediate(toolkitObject);
                Object.DestroyImmediate(canvasObject);
            }
        }

        [Test]
        public void MixedStackPresenterUsesItsLayoutLayer()
        {
            var sourcePanel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            Assert.That(sourcePanel, Is.Not.Null);
            var gameObject = new GameObject("Layered ViewLayout test", typeof(RectTransform));
            var layoutObject = new GameObject("Layout Canvas test", typeof(RectTransform));
            var layoutCanvas = layoutObject.AddComponent<Canvas>();
            layoutCanvas.sortingOrder = 575;
            var renderer = gameObject.AddComponent<PanelRenderer>();
            renderer.panelSettings = sourcePanel;
            var view = gameObject.AddComponent<UiToolkitEditModeTestView>();
            var layout = new ViewLayout { LayerType = ViewType.Overlay, LayoutCanvas = layoutCanvas };

            try
            {
                layout.Push(view);
                Assert.That(renderer.panelSettings.sortingOrder, Is.EqualTo(575));
                Assert.That(renderer.sortingOrder, Is.EqualTo(575));
                layout.Dispose();
                Assert.That(renderer.panelSettings, Is.SameAs(sourcePanel));
            }
            finally
            {
                layout.Dispose();
                Object.DestroyImmediate(gameObject);
                Object.DestroyImmediate(layoutObject);
            }
        }

        [UnityTest]
        public IEnumerator MixedStackPresenterAllowsInputForViewsWithEqualHighestOrder()
        {
            var sourcePanel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            Assert.That(sourcePanel, Is.Not.Null);
            var hostObject = new GameObject("Mixed stack host", typeof(RectTransform));
            var firstObject = new GameObject("First tied window", typeof(RectTransform));
            var secondObject = new GameObject("Second tied window", typeof(RectTransform));
            var renderer = hostObject.AddComponent<PanelRenderer>();
            renderer.panelSettings = sourcePanel;
            var host = hostObject.AddComponent<UiToolkitEditModeTestView>();
            var firstCanvas = firstObject.AddComponent<Canvas>();
            var secondCanvas = secondObject.AddComponent<Canvas>();
            var first = firstObject.AddComponent<CanvasEditModeTestView>();
            var second = secondObject.AddComponent<CanvasEditModeTestView>();
            var layoutObject = new GameObject("Window Layout Canvas test", typeof(RectTransform));
            var layoutCanvas = layoutObject.AddComponent<Canvas>();
            layoutCanvas.sortingOrder = 350;
            var layout = new ViewLayout { LayerType = ViewType.Window, LayoutCanvas = layoutCanvas };

            try
            {
                yield return first.Initialize(new EmptyViewModel()).ToCoroutine();
                yield return second.Initialize(new EmptyViewModel()).ToCoroutine();
                layout.Push(host);
                layout.Push(first);
                layout.Push(second);
                yield return first.ShowAsync().ToCoroutine();
                yield return second.ShowAsync().ToCoroutine();
                firstObject.GetComponent<UiToolkitMixedOrderAdapter>().SetRequestedOrder(2);

                Assert.That(firstCanvas.sortingOrder, Is.EqualTo(352));
                Assert.That(secondCanvas.sortingOrder, Is.EqualTo(352));
                Assert.That(firstObject.GetComponent<CanvasGroup>().blocksRaycasts, Is.True);
                Assert.That(secondObject.GetComponent<CanvasGroup>().blocksRaycasts, Is.True);
            }
            finally
            {
                layout.Dispose();
                Object.DestroyImmediate(hostObject);
                Object.DestroyImmediate(firstObject);
                Object.DestroyImmediate(secondObject);
                Object.DestroyImmediate(layoutObject);
            }
        }

        [UnityTest]
        public IEnumerator PanelRendererIsSupportedAsNonInteractiveWorldspaceHost() =>
            VerifyPanelRenderer().ToCoroutine();

        private static async UniTask VerifyPanelRenderer()
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreePath);
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            Assert.That(tree, Is.Not.Null);
            Assert.That(panelSettings, Is.Not.Null);

            var gameObject = new GameObject("UITK PanelRenderer view test");
            gameObject.SetActive(false);
            var renderer = gameObject.AddComponent<PanelRenderer>();
            renderer.visualTreeAsset = tree;
            renderer.panelSettings = panelSettings;
            renderer.worldSpaceSizeMode = WorldSpaceSizeMode.Dynamic;
            var view = gameObject.AddComponent<UiToolkitEditModeTestView>();
            view.acceptInput = false;
            try
            {
                gameObject.SetActive(true);
                await view.Initialize(new EmptyViewModel());
                Assert.That(view.HasActiveHost, Is.True);
                Assert.That(view.IsToolkitReady, Is.True);
                Assert.That(view.Root.pickingMode, Is.EqualTo(PickingMode.Ignore));
                var content = view.Root.Q<VisualElement>(className: UiToolkitEditModeTestView.ContentClassName);
                Assert.That(content.ClassListContains(UiToolkitEditModeTestView.HiddenClassName), Is.True);
                view.SetVisible(true);
                Assert.That(content.ClassListContains(UiToolkitEditModeTestView.HiddenClassName), Is.False);
                view.SetVisible(false);
                Assert.That(content.ClassListContains(UiToolkitEditModeTestView.HiddenClassName), Is.True);
                Assert.That(renderer.forceRenderingOff, Is.False);
                view.Destroy();
            }
            finally { Object.DestroyImmediate(gameObject); }
        }
    }

    public sealed class UiToolkitEditModeTestView : UiToolkitView<EmptyViewModel> { }
    public sealed class CanvasEditModeTestView : View<EmptyViewModel> { }
}
#endif
