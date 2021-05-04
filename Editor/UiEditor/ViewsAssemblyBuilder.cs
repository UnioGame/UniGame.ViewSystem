using System;
using UniGame.UiSystem.Runtime;
using UniModules.UniCore.EditorTools.Editor.Utility;
using UniModules.UniGame.AddressableExtensions.Editor;
using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;
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

    public class ViewsAssemblyBuilder
    {
        private Type baseViewType  = typeof(IUiView<>);
        private Type baseModelType = typeof(IViewModel);
        private HashSet<IView> proceedViews = new HashSet<IView>();
        private HashSet<UiViewReference> viewsReferences = new HashSet<UiViewReference>();
        private List<ViewModelFactorySettings> contextViewsMapSettings = new List<ViewModelFactorySettings>();
        private AddressableAssetSettings addressableAssetSettings;
        private List<Action> rebuildCommands;
        private List<Func<ViewsSettings,bool>> rebuildSettingsCommands;

        public ViewsAssemblyBuilder()
        {
            rebuildCommands = new List<Action>()
            {
                Reset,
                RebuildViewSettings,
                RebuildContextSettings,
            };

            rebuildSettingsCommands = new List<Func<ViewsSettings, bool>>()
            {
                Clear,
                ValidateSettings,
                BuildViewSettingsData,
            };
        }
        
        public void RebuildAll()
        {
            rebuildCommands.ForEach(x => x.Invoke());
        }

        public void RebuildViewSettings()
        {
            var viewSettings = AssetEditorTools.GetAssets<ViewsSettings>();
            foreach (var setting in viewSettings)
            {
                Build(setting);
                setting.MarkDirty();
            }
        }

        public void Build(ViewsSettings settings)
        {
            addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
            
            if (settings.isActive == false) return;

            proceedViews.Clear();
            
            if (!settings) {
                GameLog.LogError($"EMPTY UiManagerSettings on UiAssemblyBuilder.Build");
                return;
            }

            ApplyViewSettingsPipeline(settings);
            
            settings.MarkDirty();
        }

        public bool ValidateSettings(ViewsSettings settings)
        {
            return settings.IsActive;
        }
        
        public bool Clear(ViewsSettings settings)
        {
            settings.uiViews.Clear();
            return settings;
        }

        private HashSet<ViewsSettings> GetSettings(ViewsSettings viewsSettings, HashSet<ViewsSettings> settings)
        {
            if (!settings.Add(viewsSettings))
                return settings;

            if (!(viewsSettings is ViewSystemSettings systemSettings)) return settings;
            
            var nestedSettings = systemSettings.sources;
            foreach (var nestedSetting in nestedSettings)
            {
                GetSettings(nestedSetting.viewSourceReference.editorAsset,settings);
            }

            return settings;
        }
        
        private ViewsSettings ApplyViewSettingsPipeline(ViewsSettings settings)
        {
            foreach (var settingsCommand in rebuildSettingsCommands)
            {
                var result = settingsCommand(settings);
                if (!result) break;
            }

            return settings;
        }

        private bool BuildViewSettingsData(ViewsSettings settings)
        {
            settings.uiViews.Clear();
            
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

            return settings;
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

            var viewReference = CreateViewReference(view, groupName, defaultView);
            
            viewsReferences.Add(viewReference);
            views.Add(viewReference);
        }

        public void Reset()
        {
            proceedViews.Clear();
            viewsReferences.Clear();
            contextViewsMapSettings.Clear();
        }
        
        public UiViewReference CreateViewReference(
            IView view, 
            string groupName,bool defaultView)
        {
            var assetView = view as MonoBehaviour;
            var gameObject = assetView.gameObject;
            var assetPath = AssetDatabase.GetAssetPath(gameObject);
            var tag = defaultView ? string.Empty : Path.GetFileName(Path.GetDirectoryName(assetPath));

            gameObject.SetAddressableAssetGroup(groupName);
            var assetReference = gameObject.PrefabToAssetReference();
            
            if (assetReference.RuntimeKeyIsValid() == false) {
                GameLog.LogError($"Asset {gameObject.name} by path {assetPath} wrong addressable asset");
                return null;
            }
            
            var viewType = view.GetType();
            var viewInterface = viewType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == baseViewType);
            
            var modelsArgs = viewInterface?.GetGenericArguments();
            var modelType = modelsArgs?.FirstOrDefault();

            var viewDescription = new UiViewReference() {
                Tag  = tag,
                AssetGUID = assetReference.AssetGUID,
                Type = viewType,
                ModelType = modelType,
                View = assetReference,
                ViewName = assetReference.editorAsset.name
            };

            return viewDescription;
        }
        
        
        private void RebuildContextSettings()
        {
            var contextSettings = AssetEditorTools.GetAssets<ViewModelFactorySettings>();
            
        }

    }
}