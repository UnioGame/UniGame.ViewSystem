using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Taktika.UI.Editor.UiEditor;
using UniGreenModules.UniCore.EditorTools.Editor.AssetOperations;
using UniGreenModules.UniCore.EditorTools.Editor.Utility;
using UniGreenModules.UniGame.UiSystem.Runtime.Settings;
using UnityEditor;

namespace UniGame.UiSystem.Editor.PostProcessors
{
    public class UpdateSystemSettingsProcessor : AssetPostprocessor
    {
        private static Dictionary<UiViewsSource,List<string>> uiSystemSettings = new Dictionary<UiViewsSource,List<string>>(8);

        private static HashSet<UiViewsSource> settingsToRebuild = new HashSet<UiViewsSource>();
        
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            BuildSettingsData();
            
            foreach (string assetPath in importedAssets)
            {
                Validate(assetPath);
            }
            
            foreach (string assetPath in deletedAssets)
            {
                Validate(assetPath);
            }

            foreach (var assetPath in movedAssets)
            {
                Validate(assetPath);
            }
            
            Rebuild();
        }

        private static void Rebuild()
        {
            foreach (var uiViewsSource in settingsToRebuild)
            {
                uiViewsSource.Build();
                EditorUtility.SetDirty(uiViewsSource);
            }
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
            uiSystemSettings.Clear();
            settingsToRebuild.Clear();
            
            var settings = AssetEditorTools.GetAssets<UiViewsSource>();

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
