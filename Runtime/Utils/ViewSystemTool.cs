namespace UniGame.UISystem.Runtime.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using UniGame.UiSystem.Runtime.Settings;
    
#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
    public static class ViewSystemTool
    {
        public static IEnumerable<string> GetViewNames()
        {
#if UNITY_EDITOR
            var settings = AssetEditorTools.GetAsset<ViewSystemSettings>();
            return settings.GetEditorViewsId();
#endif
            return Enumerable.Empty<string>();
        }
    }
}