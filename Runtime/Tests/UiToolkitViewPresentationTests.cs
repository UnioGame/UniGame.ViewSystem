namespace UniGame.UiSystem.Runtime.UiToolkit.Tests
{
    using System.Reflection;
    using NUnit.Framework;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public sealed class UiToolkitViewPresentationTests
    {
        private const string VisualTreePath = "Assets/Game.Content/Ui/UIToolkitSandbox/UXML/UIToolkitSmokeOverlay.uxml";
        private const string PanelSettingsPath = "Assets/Game.Content/Ui/_Settings/UIDefaultPanelSettings-Overlay.asset";
        private const string WorldspaceVisualTreePath = "Assets/Game.Content/Ui/WorldspaceUIToolkit/BarkView.uxml";
        private const string WorldspacePanelSettingsPath = "Assets/Game.Content/Ui/_Settings/UIDefaultPanelSettings-Worldspace.asset";

        [Test]
        public void PanelRendererIsSupportedAsNonInteractiveWorldspaceHost()
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(WorldspaceVisualTreePath);
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(WorldspacePanelSettingsPath);
            Assert.That(tree, Is.Not.Null);
            Assert.That(panelSettings, Is.Not.Null);

            var gameObject = new GameObject("UITK PanelRenderer presentation test");
            gameObject.SetActive(false);
            var renderer = gameObject.AddComponent<PanelRenderer>();
            renderer.visualTreeAsset = tree;
            renderer.panelSettings = panelSettings;
            renderer.worldSpaceSizeMode = WorldSpaceSizeMode.Dynamic;
            var presentation = gameObject.AddComponent<UiToolkitViewPresentation>();
            presentation.panelRenderer = renderer;
            presentation.acceptInput = false;
            gameObject.SetActive(true);
            try
            {
                presentation.Initialize("PanelRendererTest");

                Assert.That(presentation.HasActiveHost, Is.True);
                Assert.That(presentation.IsReady, Is.True);
                Assert.That(presentation.Root, Is.Not.Null);
                Assert.That(presentation.Root.pickingMode, Is.EqualTo(PickingMode.Ignore));
                var content = presentation.Root.Q<VisualElement>(className: UiToolkitViewPresentation.ContentClassName);
                Assert.That(content.ClassListContains(UiToolkitViewPresentation.HiddenClassName), Is.True);
                presentation.SetVisible(true);
                Assert.That(content.ClassListContains(UiToolkitViewPresentation.HiddenClassName), Is.False);
                presentation.SetVisible(false);
                Assert.That(content.ClassListContains(UiToolkitViewPresentation.HiddenClassName), Is.True);
                Assert.That(renderer.forceRenderingOff, Is.False);
            }
            finally { Object.DestroyImmediate(gameObject); }
        }

        [Test]
        public void BindingLifetimeIsReleasedExactlyOnceAcrossTwentyPoolCycles()
        {
            var gameObject = CreateConfiguredPanelRenderer("UITK presentation test", out var presentation);
            try
            {
                var released = 0;
                for (var cycle = 0; cycle < 20; cycle++)
                {
                    presentation.Initialize();
                    presentation.AddUnbind(() => released++);
                    presentation.ClearBindings();
                    presentation.ClearBindings();
                    Assert.That(released, Is.EqualTo(cycle + 1));
                }
            }
            finally { Object.DestroyImmediate(gameObject); }
        }

        [Test]
        public void DisableEnableRefreshesTheRuntimeTreeAndDropsOldBindings()
        {
            var gameObject = CreateConfiguredPanelRenderer("UITK tree test", out var presentation);
            try
            {
                presentation.Initialize();
                var first = presentation.Root;
                var released = 0;
                presentation.AddUnbind(() => released++);
                typeof(UiToolkitViewPresentation).GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(presentation, null);
                gameObject.SetActive(false);
                Assert.That(released, Is.EqualTo(1));
                Assert.That(presentation.IsReady, Is.False);
                gameObject.SetActive(true);
                presentation.RefreshTree();
                Assert.That(presentation.IsReady, Is.True);
                Assert.That(presentation.Root, Is.Not.Null);
                Assert.That(first, Is.Not.Null);
            }
            finally { Object.DestroyImmediate(gameObject); }
        }

        private static GameObject CreateConfiguredPanelRenderer(string name, out UiToolkitViewPresentation presentation)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreePath);
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            Assert.That(visualTree, Is.Not.Null, $"Missing UITK test VisualTreeAsset at '{VisualTreePath}'.");
            Assert.That(panelSettings, Is.Not.Null, $"Missing UITK test PanelSettings at '{PanelSettingsPath}'.");

            var gameObject = new GameObject(name);
            gameObject.SetActive(false);
            var renderer = gameObject.AddComponent<PanelRenderer>();
            presentation = gameObject.AddComponent<UiToolkitViewPresentation>();
            renderer.visualTreeAsset = visualTree;
            renderer.panelSettings = panelSettings;
            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
