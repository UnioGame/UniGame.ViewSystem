using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UniGreenModules.UniCore.EditorTools.Editor.AssetOperations;
using UnityEditor;

namespace UniGame.UiSystem.Editor.PostProcessors
{
    using Runtime.Settings;
    using UI.Editor.UiEdito;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.EditorTools.Editor.Utility;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Utils;
    using UnityEngine;

    public class UpdateSystemSettingsProcessor : UnityEditor.AssetModificationProcessor
    {
        private static Func<object, List<ViewsSettings>> _viewsSettingsCache = MemorizeTool.
            Create<object, List<ViewsSettings>>(x => AssetEditorTools.GetAssets<ViewsSettings>());
        
        static string[] OnWillSaveAssets(string[] paths)
        {
            return paths;
            var assets = _viewsSettingsCache(string.Empty);
            foreach (var asset in assets)
            {
                if(!Validate(asset,paths))
                    continue;
                
                Rebuild(asset);
            }

            return paths;
        }

        public static void Rebuild(ViewsSettings settings)
        {
            settings.Build();
            settings.SetDirty();
            GameLog.Log($"Rebuild Ui View Settings",Color.blue);
        }

        private static bool Validate(ViewsSettings settings,string[] paths)
        {
            var settingsTargets = GetSettingsPath(settings);
            var changesViews  = paths.
                Any(x => settingsTargets.
                    Any(path => x.IndexOf(path,StringComparison.OrdinalIgnoreCase) >= 0));
            return changesViews;
        }

        private static IEnumerable<string> GetSettingsPath(ViewsSettings settings)
        {
            return settings.uiViewsDefaultFolders.
                Concat(settings.uiViewsSkinFolders);
        }
    }

}
