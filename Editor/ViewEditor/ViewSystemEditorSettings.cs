using UniGame.UiSystem.Runtime;
using UniGame.UiSystem.Runtime.Settings;
using UniGame.UiSystem.Runtime.ViewsFlow;
using UniModules.Editor;

namespace UniModules.UniGame.ViewSystem
{
    using System;
    using System.Collections.Generic;
    using global::UniGame.ViewSystem.Runtime;
    using UniModules.Editor;
    using global::UniGame.Attributes;
    using global::UniGame.ViewSystem.Runtime.WindowStackControllers.Abstract;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Object = UnityEngine.Object;
    
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Editor/Editor Template ViewSystemSettings",fileName = nameof(ViewSystemEditorSettings))]
    public class ViewSystemEditorSettings : ScriptableObject
    {
        public Object prototypeFolder;

        public ViewSystemSettings viewSystemSettingsAsset;

        public GameObject viewPrefab;

        public ViewModelResolverSettings viewModelResolver;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ValueDropdown(nameof(GetFlowTypes))]
#endif
        public ViewFlowControllerAsset defaultFlowAsset;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Required]
#endif
        [AssetFilter(typeof(ViewLayoutType))]
        public ViewLayoutType defaultLayoutType;

        
        public List<ViewFlowControllerAsset> GetFlowTypes()
        {
            return AssetEditorTools.GetAssets<ViewFlowControllerAsset>();
        }
        
        #region static data

        private static Type defaultType = typeof(DefaultAsset);
        
        private static ViewSystemEditorSettings _viewSystemEditorSettings;

        public static ViewSystemEditorSettings ViewEditorSettings
        {
            get
            {
                if (_viewSystemEditorSettings) return _viewSystemEditorSettings;
                _viewSystemEditorSettings = AssetEditorTools.GetAsset<ViewSystemEditorSettings>();
                return _viewSystemEditorSettings;
            }
        }


        [MenuItem("Assets/UniGame/ViewSystem/Create ViewSystem Prefab")]
        public static void CreateViewSystemPrefab()
        {
            var activeObject = Selection.activeObject;
            if (!activeObject) return;
            
            var path = AssetDatabase.GetAssetPath(activeObject);
            path = path.GetDirectoryPath();

            Debug.Log($"ASSET PATH SELECTION :  {path}");
            
            CreateViewAssets(path);
        }

        public static void CreateViewAssets(string path)
        {
            var viewSystemPrefab = ViewEditorSettings.viewPrefab;
            var settings = ViewEditorSettings.viewSystemSettingsAsset;

            var view = viewSystemPrefab.CopyAsset<GameViewSystemAsset>(viewSystemPrefab.name,path);
            var settingsAsset = settings.CopyAsset<ViewSystemSettings>(settings.name, path);

            settingsAsset.isActive = true;
            view.settings = new AssetReferenceT<ViewSystemSettings>(settingsAsset.GetGUID());

            view.gameObject.MarkDirty();
            settingsAsset.MarkDirty();
            
            AssetDatabase.Refresh();
        }

        #endregion
    }
}

