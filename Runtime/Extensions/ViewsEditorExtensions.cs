namespace Extensions
{
    using System;
    using UniGame.UiSystem.Runtime.Settings;
    using UnityEngine;


#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
    public static class ViewsEditorExtensions
    {
        
        public static GameObject PingView(this Type viewType)
        {
#if UNITY_EDITOR
            var viewSettings = AssetEditorTools.GetAssets<ViewsSettings>();
            foreach (var viewSetting in viewSettings)
            {
                foreach (var uiViewReference in viewSetting.uiViews)
                {
                    if(uiViewReference.Type != viewType) continue;
                    var asset = uiViewReference.View.editorAsset;
                    if(asset==null) continue;

                    asset.PingInEditor();
                    return asset;
                }
            }

            return null;
#endif
            return null;
        }
        
    }
}