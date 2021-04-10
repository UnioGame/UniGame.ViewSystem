using UniModules.UniCore.EditorTools.Editor.Utility;
using UniModules.UniGame.AddressableExtensions.Editor;
using UnityEngine;

namespace UniGame.UiSystem.Editor.UiEditor
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Runtime.Settings;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniGame.Core.EditorTools.Editor.AssetOperations;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine.AddressableAssets;

    public class ViewsAssemblyBuilder
    {
        private HashSet<IView> proceedViews = new HashSet<IView>();
        private AddressableAssetSettings addressableAssetSettings;
        
        public void Build(ViewsSettings settings)
        {
            addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings.isActive == false) return;

            proceedViews.Clear();
            
            if (!settings) {
                GameLog.LogError($"EMPTY UiManagerSettings on UiAssemblyBuilder.Build");
                return;
            }
            
            Reset(settings);

            var skinsFolders = settings.uiViewsSkinFolders;
            var defaultFolders = settings.uiViewsDefaultFolders;
            var groupName = string.IsNullOrEmpty(settings.sourceName) ? 
                settings.name : settings.sourceName;
            
            if (skinsFolders.Count > 0) {
                var views = LoadUiViews<IView>(skinsFolders);
                views.ForEach(x => AddView(settings,x,false,groupName));
            }

            if (defaultFolders.Count > 0) {
                var views = LoadUiViews<IView>(defaultFolders);
                views.ForEach(x => AddView(settings,x,true,groupName));
            }
            
            settings.MarkDirty();
        }

        public void Reset(ViewsSettings settings)
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


        private void AddView(ViewsSettings viewsSettings,IView view, bool defaultView, string groupName)
        {
            var assetView = view as MonoBehaviour;
            if (assetView == null) {
                GameLog.LogError($"View at Path not Unity Asset with View Type {defaultView}");
                return;
            }

            var gameObject = assetView.gameObject;
            var guid = gameObject.GetGUID();
            var views = viewsSettings.uiViews;
            
            if (views.Any(x => string.Equals(guid, x.AssetGUID)))
                return;
            
            var assetPath = AssetDatabase.GetAssetPath(gameObject);
            var tag = defaultView ? string.Empty : Path.GetFileName(Path.GetDirectoryName(assetPath));
            var labels = viewsSettings.labels;

            gameObject.SetAddressableAssetGroup(groupName);
            var assetReference = gameObject.PrefabToAssetReference();
            
            if (assetReference.RuntimeKeyIsValid() == false) {
                GameLog.LogError($"Asset {gameObject.name} by path {assetPath} wrong addressable asset");
                return;
            }

            var viewDescription = new UiViewReference() {
                Tag  = tag,
                AssetGUID = assetReference.AssetGUID,
                Type = view.GetType(),
                View = assetReference,
                ViewName = assetReference.editorAsset.name
            };

            views.Add(viewDescription);
        }
    }
}