using UniModules.UniGame.ViewSystem.ModelViews.Editor;

namespace UniGame.UiSystem.ModelViews.Editor.PostProcessors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ModelViewsMap.Runtime.Settings;
    using UiSystem.Runtime;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.Editor;
    using ViewSystem.Runtime;
    using UnityEditor;

    public class UpdateModelViewsSettingsProcessor : AssetModificationProcessor
    {
        private const string cacheKey = "";
        
        public static Func<object,List<ModelViewsModuleSettings>> settingsCache = MemorizeTool.
            Create<object,List<ModelViewsModuleSettings>>(x => AssetEditorTools.
            GetAssets<ModelViewsModuleSettings>());
        
        public static string[] OnWillSaveAssets(string[] paths)
        {
            return paths;
            
            var settingsAssets = settingsCache(cacheKey);
            
            foreach (var asset in settingsAssets) {
                if(!ValidateTarget(asset,paths))
                    continue;
                ModelViewsEditorCommands.Rebuild(asset);
            }
            
            return paths;
        }        
  
        private static bool ValidateTarget(ModelViewsModuleSettings asset, string[] paths)
        {
            if (!asset || asset.isRebuildActive == false) return false;
            if (asset.updateTargets.Count == 0) return true;
            
            foreach (var targetPath in asset.updateTargets) {
                if (paths.Any(x => x.IndexOf(targetPath, StringComparison.OrdinalIgnoreCase) >= 0)) {
                    return true;
                }
            }

            return false;
        }

    }
}
