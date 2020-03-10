namespace Taktika.UI.Editor.UiEditor
{
    using UniGreenModules.UniCore.EditorTools.Editor.AssetOperations;
    using UniGreenModules.UniGame.UiSystem.Runtime.Settings;
    using UnityEditor;

    public class UiAssembler 
    {
        private static UiAssemblyBuilder settingsBuilder = new UiAssemblyBuilder();
        
        [MenuItem(itemName:"Taktika/UI/Refresh UI Views")]
        public static void RefreshUiSettings()
        {

            var uiSettings = AssetEditorTools.GetAssets<UiViewsSource>();
            
            foreach (var source in uiSettings) {
                Build(source);
            }
            
        }

        public static void Build(UiViewsSource source)
        {
            if (source == null)
                return;
            settingsBuilder.Build(source);
        }

        [MenuItem(itemName:"Assets/Rebuild UI Source")]
        public static void RebuildSelected()
        {
            Build(Selection.activeObject as UiViewsSource);
        }
        
    }
}
