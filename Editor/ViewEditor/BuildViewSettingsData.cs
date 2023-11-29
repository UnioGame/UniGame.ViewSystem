﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniCore.Runtime.ProfilerTools;
using UniGame.UiSystem.Runtime.Settings;
using UniModules.Editor;
using UniModules.UniGame.AddressableExtensions.Editor;
using UniGame.ViewSystem.Runtime;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.ViewSystem
{
    public class BuildViewSettingsData : IViewAssemblerCommand
    {
        private HashSet<IView> proceedViews = new HashSet<IView>();
        private List<UiViewReference> previousReferences = new List<UiViewReference>();

        public bool Execute(ViewsSettings settings)
        {
            proceedViews.Clear();

            previousReferences.Clear();
            previousReferences.AddRange(settings.uiViews);
            settings.uiViews.Clear();

            var skinsFolders = settings.uiViewsSkinFolders;
            var defaultFolders = settings.uiViewsDefaultFolders;
            var assetsSources = settings.viewsAssetsSources;

            var groupName = string.IsNullOrEmpty(settings.sourceName)
                ? settings.name
                : settings.sourceName;

            if (skinsFolders.Count > 0)
            {
                var views = LoadUiViews<IView>(skinsFolders);
                views.ForEach(x => AddView(settings, x, false, groupName));
            }

            if (defaultFolders.Count > 0)
            {
                var views = LoadUiViews<IView>(defaultFolders);
                views.ForEach(x => AddView(settings, x, true, groupName));
            }

            if (assetsSources.Count > 0)
            {
                var views = assetsSources.Where(x => x.editorAsset != null)
                    .Select(x => x.editorAsset)
                    .Select(x => x.GetComponent<IView>())
                    .Where(x => x != null)
                    .ToList();
                
                views.ForEach(x => AddView(settings, x, true, groupName));
            }

            foreach (var uiView in settings.uiViews)
                ApplyOverrideValues(uiView,previousReferences);

            previousReferences.Clear();
            return settings;
        }

        public UiViewReference CreateViewReference(IView view, bool defaultView,bool overrideAddressableGroup,string groupName)
        {
            var viewDescription = new UiViewReference();
            UpdateViewReferenceData(viewDescription, view, defaultView,overrideAddressableGroup, groupName);
            return viewDescription;
        }

        public UiViewReference UpdateViewReferenceData(
            UiViewReference viewDescription,
            bool defaultView = true, 
            bool overrideAddressableGroup = false,
            string addressableGroup = null)
        {
            var viewObject = viewDescription.View.editorAsset;
            if (viewObject == null)
            {
                Debug.LogError($"View System Error: NULL VIEW OBJECT FOUND ON REBUILD {viewDescription.ViewName}");
                return viewDescription;
            }

            var view = viewObject.GetComponent<IView>();
            if (view == null)
            {
                Debug.LogError($"View System Error: NON OBJECT VIEW FOUND ON REBUILD {viewDescription.ViewName} {viewObject}");
                return viewDescription;
            }

            return UpdateViewReferenceData(viewDescription, 
                view, defaultView,
                overrideAddressableGroup, addressableGroup);
        }

        public UiViewReference UpdateViewReferenceData(UiViewReference viewDescription, 
            IView view, bool defaultView = true,
            bool overrideAddressableGroup = false,
            string addressableGroup = null)
        {
            var gameObject = view.GameObject;
            var assetPath = AssetDatabase.GetAssetPath(gameObject);
            var tag = GetSkinTag(view, defaultView);

            ApplyViewAddressable(gameObject, overrideAddressableGroup, addressableGroup);

            var assetReference = gameObject.PrefabToAssetReference();
            if (assetReference.RuntimeKeyIsValid() == false)
            {
                GameLog.LogError($"Asset {gameObject.name} by path {assetPath} wrong addressable asset");
                return null;
            }
            
            var guid = assetReference.AssetGUID;
            var viewType = view.GetType();
            var modelType = ViewSystemUtils.GetModelTypeByView(viewType);
            var viewModelType = ViewSystemUtils.GetFirstAssignable(modelType);
            
            viewDescription.Tag = tag;
            viewDescription.AssetGUID = guid;
            viewDescription.Type = viewType;
            viewDescription.ModelType = modelType;
            viewDescription.ViewModelType = viewModelType;
            viewDescription.View = assetReference;
            viewDescription.ViewName = assetReference.editorAsset.name;
            viewDescription.PoolingPreloadCount = viewDescription.PoolingPreloadCount;
            viewDescription.Hash = viewDescription.GetHashCode();
            
            return viewDescription;
        }

        public void ApplyViewAddressable(
            GameObject gameObject,
            bool overrideAddressableGroup = false,
            string addressableGroup = null)
        {
            if(overrideAddressableGroup && !string.IsNullOrEmpty(addressableGroup))
                gameObject.SetAddressableAssetGroup(addressableGroup);

            if (gameObject.IsInAnyAddressableAssetGroup()) return;
            
            if (string.IsNullOrEmpty(addressableGroup))
            {
                gameObject.AddToDefaultAddressableGroup();
            }
            else
            {
                gameObject.SetAddressableAssetGroup(addressableGroup);
            }
        }
        
        public void Reset() => proceedViews.Clear();

        public string GetSkinTag(IView view, bool defaultView)
        {
            if (!(view is MonoBehaviour assetView))
                return string.Empty;

            var gameObject = assetView.gameObject;

            var skinOverride = gameObject.GetComponent<IViewSkinTag>();

            if (defaultView)
                return skinOverride == null
                    ? string.Empty
                    : skinOverride.SkinTag;

            var assetPath = AssetDatabase.GetAssetPath(gameObject);
            return Path.GetFileName(Path.GetDirectoryName(assetPath));
        }

        private List<TView> LoadUiViews<TView>(IReadOnlyList<string> paths)
            where TView : class, IView
        {
            var assets = AssetEditorTools.GetAssets<GameObject>(paths.ToArray());

            var views = assets
                .Select(x => x.GetComponent<TView>())
                .Where(x => x != null)
                .Where(x => !proceedViews.Contains(x)).ToList();

            return views;
        }

        private void ApplyOverrideValues(UiViewReference viewReference,List<UiViewReference> values)
        {
            var overrideValue = values
                .FirstOrDefault(x => x.Hash == viewReference.Hash);
            
            if (overrideValue == null) return;
            
            var type = overrideValue.ViewModelType.Type;
            if (type is { IsAbstract: false, IsInterface: false })
            {
                viewReference.ViewModelType = overrideValue.ViewModelType;
            }
            
            viewReference.PoolingPreloadCount = overrideValue.PoolingPreloadCount;
        }

        private void AddView(ViewsSettings settings, IView view, bool defaultView, string groupName)
        {
            var views = settings.uiViews;
            var assetView = view as MonoBehaviour;
            if (assetView == null)
            {
                GameLog.LogError($"View at group : {groupName} not Unity Asset with View Type {defaultView}");
                return;
            }

            var gameObject = assetView.gameObject;
            var guid = gameObject.GetGUID();

            if (views.Any(x => string.Equals(guid, x.AssetGUID)))
                return;

            var viewReference = CreateViewReference(view, defaultView, 
                settings.applyAddressablesGroup, groupName);
            
            views.Add(viewReference);
        }
    }
}