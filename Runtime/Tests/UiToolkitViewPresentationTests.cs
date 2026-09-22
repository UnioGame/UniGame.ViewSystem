namespace UniGame.UiSystem.Runtime.UiToolkit.Tests
{
    using System.Reflection;
    using NUnit.Framework;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public sealed class UiToolkitViewPresentationTests
    {
        private const string VisualTreePath = "Assets/GameContent/UI/UISandbox/UISandbox.uxml";
        private const string PanelSettingsPath = "Assets/GameAssets/UI Toolkit/PanelSettings.asset";

        [Test]
        public void BindingLifetimeIsReleasedExactlyOnceAcrossTwentyPoolCycles()
        {
            var gameObject = CreateConfiguredDocument("UITK presentation test", out var presentation);
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
            var gameObject = CreateConfiguredDocument("UITK tree test", out var presentation);
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

        private static GameObject CreateConfiguredDocument(string name, out UiToolkitViewPresentation presentation)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreePath);
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            Assert.That(visualTree, Is.Not.Null, $"Missing UITK test VisualTreeAsset at '{VisualTreePath}'.");
            Assert.That(panelSettings, Is.Not.Null, $"Missing UITK test PanelSettings at '{PanelSettingsPath}'.");

            var gameObject = new GameObject(name);
            gameObject.SetActive(false);
            var document = gameObject.AddComponent<UIDocument>();
            presentation = gameObject.AddComponent<UiToolkitViewPresentation>();
            document.visualTreeAsset = visualTree;
            document.panelSettings = panelSettings;
            gameObject.SetActive(true);
            return gameObject;
        }
    }
}
