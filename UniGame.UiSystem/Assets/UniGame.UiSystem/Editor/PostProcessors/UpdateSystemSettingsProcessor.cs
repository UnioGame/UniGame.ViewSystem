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
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UnityEngine;

    public class UpdateSystemSettingsProcessor : SaveAssetsProcessor
    {
        private static Dictionary<ViewsSource,List<string>> uiSystemSettings = new Dictionary<ViewsSource,List<string>>(8);

        private static HashSet<ViewsSource> settingsToRebuild = new HashSet<ViewsSource>();
        
        static string[] OnWillSaveAssets(string[] paths)
        {
            BuildSettingsData();

            foreach (var assetPath in paths)
            {
                Validate(assetPath);
            }

            Rebuild();
            
            return paths;
        }
        
//        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
//        {
//            BuildSettingsData();
//
//            var items = importedAssets.
//                Concat(deletedAssets).
//                Concat(movedAssets);
//            
//            foreach (var assetPath in items)
//            {
//                Validate(assetPath);
//            }
//
//            Rebuild();
//        }

        private static void Rebuild()
        {
            if (settingsToRebuild.Count == 0)
                return;
            
            foreach (var uiViewsSource in settingsToRebuild)
            {
                uiViewsSource.Build();
                EditorUtility.SetDirty(uiViewsSource);
            }
            
            GameLog.Log($"Rebuild Ui View Settings",Color.blue);
        }

        private static void Validate(string assetPath)
        {
            var changesViews  = uiSystemSettings.
                Where(x => x.Value.
                    Any(path => assetPath.IndexOf(path,StringComparison.OrdinalIgnoreCase) >= 0)).
                Select(x => x.Key);
            settingsToRebuild.AddRange(changesViews);
        }

        private static void BuildSettingsData()
        {
            settingsToRebuild.Clear();
            if (uiSystemSettings.Count > 0)
                return;
            
            uiSystemSettings.Clear();
            
            var settings = AssetEditorTools.GetAssets<ViewsSource>();
            foreach (var uiViewsSource in settings)
            {
                var items = new List<string>();
                uiSystemSettings[uiViewsSource] = items;

                items.AddRange(uiViewsSource.uiViewsDefaultFolders);
                items.AddRange(uiViewsSource.uiViewsSkinFolders);
            }
        }
    }

}
