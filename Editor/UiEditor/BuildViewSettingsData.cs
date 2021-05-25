using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniCore.Runtime.ProfilerTools;
using UniGame.UiSystem.Runtime.Settings;
using UniModules.UniGame.AddressableExtensions.Editor;
using UniModules.UniGame.Core.EditorTools.Editor.AssetOperations;
using UniModules.UniGame.UISystem.Runtime.Abstract;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
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

            settings.uiViews
                .ForEach(ApplyOverrideValues);

            previousReferences.Clear();
            return settings;
        }

        public UiViewReference CreateViewReference(IView view, bool defaultView,bool overrideAddressables, string groupName)
        {
            var assetView = view as MonoBehaviour;
            var gameObject = assetView.gameObject;
            var assetPath = AssetDatabase.GetAssetPath(gameObject);
            var tag = defaultView ? string.Empty : Path.GetFileName(Path.GetDirectoryName(assetPath));

            if (overrideAddressables || !gameObject.IsInAnyAddressableAssetGroup())
            {
                gameObject.SetAddressableAssetGroup(groupName);
            }
            
            var assetReference = gameObject.PrefabToAssetReference();

            if (assetReference.RuntimeKeyIsValid() == false)
            {
                GameLog.LogError($"Asset {gameObject.name} by path {assetPath} wrong addressable asset");
                return null;
            }

            var viewType = view.GetType();
            var viewInterface = viewType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == ViewSystemConstants.BaseViewType);

            var modelsArgs = viewInterface.GetGenericArguments();
            var modelType = modelsArgs.FirstOrDefault();
            var viewModelType = ViewModelsAssemblyMap.GetFirstAssignable(modelType);
                
            var viewDescription = new UiViewReference()
            {
                Tag = tag,
                AssetGUID = assetReference.AssetGUID,
                Type = viewType,
                ModelType = modelType,
                ViewModelType = viewModelType,
                View = assetReference,
                ViewName = assetReference.editorAsset.name
            };

            return viewDescription;
        }

        public void Reset() => proceedViews.Clear();

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

        private void ApplyOverrideValues(UiViewReference viewReference)
        {
            var overrideValue = previousReferences
                .FirstOrDefault(x => x.Type.Equals(viewReference.Type) &&
                                     x.ModelType.Equals(viewReference.ModelType) &&
                                     string.Equals(x.Tag, viewReference.Tag) &&
                                     string.Equals(x.ViewName, viewReference.ViewName));

            var type = overrideValue?.ViewModelType.Type;
            if (type == null || type.IsAbstract || type.IsInterface) return;

            viewReference.ViewModelType = overrideValue.ViewModelType;
        }

        private void AddView(ViewsSettings settings, IView view, bool defaultView, string groupName)
        {
            var views     = settings.uiViews;
            var assetView = view as MonoBehaviour;
            if (assetView == null)
            {
                GameLog.LogError($"View at Path not Unity Asset with View Type {defaultView}");
                return;
            }

            var gameObject = assetView.gameObject;
            var guid = gameObject.GetGUID();

            if (views.Any(x => string.Equals(guid, x.AssetGUID)))
                return;

            var viewReference = CreateViewReference(view,defaultView,settings.applyAddressablesGroup, groupName);
            views.Add(viewReference);
        }
    }
}