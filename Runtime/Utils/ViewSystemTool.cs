namespace UniGame.UISystem.Runtime.Utils
{
    using System.Collections.Generic;
    using UniGame.UiSystem.Runtime.Settings;
    
#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
    public static class ViewSystemTool
    {
        public static IEnumerable<string> GetViewNames()
        {
#if UNITY_EDITOR
            var settings = AssetEditorTools.GetAssets<ViewSystemSettings>();

            foreach (var data in settings)
            {
                if(data == null) continue;
                
                foreach (var viewId in data.GetEditorViewsId())
                {
                    yield return viewId;
                }
            }
#endif
            yield break;
        }
    }
}