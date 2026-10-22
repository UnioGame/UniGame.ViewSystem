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
}
