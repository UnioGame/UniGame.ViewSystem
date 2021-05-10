using UniModules.UniGame.ViewSystem.Editor.UiEditor;

namespace UniGame.UiSystem.UI.Editor.UiEdito
{
    using Runtime.Settings;
    using UniModules.UniGame.Core.EditorTools.Editor.AssetOperations;
    using UnityEditor;

    public static class ViewAssembler 
    {
        private static ViewsAssemblyBuilder settingsBuilder = new ViewsAssemblyBuilder();
        
        [MenuItem(itemName:"UniGame/View System/Rebuild View Settings")]
        public static void RefreshUiSettings()
        {
            settingsBuilder.RebuildAll();
        }

        public static void Build(this ViewsSettings settings)
        {
            if (settings == null)
                return;
            settingsBuilder.Build(settings);
        }

        
        [MenuItem(itemName:"Assets/Rebuild ViewsSettings")]
        public static void RebuildSelected()
        {
            Build(Selection.activeObject as ViewsSettings);
        }
    }
}
