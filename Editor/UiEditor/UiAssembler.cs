namespace UniGame.UiSystem.UI.Editor.UiEdito
{
    using Runtime.Settings;
    using UiSystem.Editor.UiEditor;
    using UniModules.UniGame.Core.EditorTools.Editor.AssetOperations;
    using UnityEditor;

    public static class UiAssembler 
    {
        private static ViewsAssemblyBuilder settingsBuilder = new ViewsAssemblyBuilder();
        
        [MenuItem(itemName:"UniGame/View System/Rebuild View Settings")]
        public static void RefreshUiSettings()
        {

            var uiSettings = AssetEditorTools.GetAssets<ViewsSettings>();
            
            foreach (var source in uiSettings) {
                Build(source);
            }
            
        }

        public static void Build(this ViewsSettings settings)
        {
            if (settings == null)
                return;
            settingsBuilder.Build(settings);
        }

        [MenuItem(itemName:"Assets/Rebuild Views Settings")]
        public static void RebuildSelected()
        {
            Build(Selection.activeObject as ViewsSettings);
        }
        
    }
}
