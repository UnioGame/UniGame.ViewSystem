using UnityEngine;

namespace Taktika.UI.Editor.UiEditor
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UniGreenModules.UniCore.EditorTools.Editor.AssetOperations;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime.Settings;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine.AddressableAssets;

    public class UiAssemblyBuilder
    {
        private HashSet<IView> proceedViews = new HashSet<IView>();
        private AddressableAssetSettings addressableAssetSettings;
        
        public void Build(UiViewsSource settings)
        {
            addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
            
            proceedViews.Clear();
            
            if (!settings) {
                GameLog.LogError($"EMPTY UiManagerSettings on UiAssemblyBuilder.Build");
                return;
            }
            
            Reset(settings);

            var skinsFolders = settings.uiViewsSkinFolders;
            var defaultFolders = settings.uiViewsDefaultFolders;

            if (skinsFolders.Count > 0) {
                var views = LoadUiViews<IView>(skinsFolders);
                views.ForEach(x => AddView(settings.uiViews,x,false));
            }

            if (defaultFolders.Count > 0) {
                var views = LoadUiViews<IView>(defaultFolders);
                views.ForEach(x => AddView(settings.uiViews,x,true));
            }
            
            settings?.SetDirty();
        }

        public void Reset(UiViewsSource settings)
        {
            settings.uiViews.Clear();
        }


        private List<TView> LoadUiViews<TView>(IReadOnlyList<string> paths)
            where TView : class,IView
        {
            var assets = AssetEditorTools.GetAssets<GameObject>(paths.ToArray());
            
            var views  = assets.
                Select(x => x.GetComponent<TView>()).
                Where(x => x != null).
                Where(x=> !proceedViews.Contains(x)).
                ToList();
            
            return views;
        }


        private void AddView(List<UiViewDescription> views,IView view, bool defaultView)
        {
            var assetView = view as MonoBehaviour;
            if (assetView == null) {
                GameLog.LogError($"View at Path not Unity Asset with View Type {defaultView}");
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(assetView.gameObject);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            
            
            var assetReference = new AssetReferenceGameObject(guid);
            if (assetReference.RuntimeKeyIsValid() == false) {
                GameLog.LogError($"Asset by path {assetPath} wrong addressable asset");
                return;
            }

            var entry = addressableAssetSettings.FindAssetEntry(guid);
            if (entry == null) {
                GameLog.LogWarning($"Add View {assetView.name} to Addressables");
                addressableAssetSettings.CreateOrMoveEntry(guid, addressableAssetSettings.DefaultGroup);
            }
            
            var viewDescription = new UiViewDescription() {
                Tag  = defaultView ? string.Empty : 
                    Path.GetFileName(Path.GetDirectoryName(assetPath)),
                Type = view.GetType(),
                View = assetReference,
            };

            if (proceedViews.Add(view)) {
                views.Add(viewDescription);
            }

        }
    }
}