using System;
using System.Collections.Generic;
using System.IO;
using UniGame.UiSystem.Runtime;
using UniGame.UiSystem.Runtime.Settings;
using UniGame.UiSystem.Runtime.ViewsFlow;
using UniModules.UniCore.EditorTools.Editor.PrefabTools;
using UniModules.UniCore.EditorTools.Editor.Utility;
using UniModules.UniGame.Core.EditorTools.Editor.AssetOperations;
using UniModules.UniGame.Core.EditorTools.Editor.Tools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Editor/ViewSystem Settings",fileName = nameof(ViewSystemEditorSettings))]
    public class ViewSystemEditorSettings : ScriptableObject
    {
        
        public Object prototypeFolder;

        public ViewSystemSettings viewSystemSettingsAsset;

        public GameObject viewPrefab;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ValueDropdown(nameof(GetFlowTypes))]
#endif
        public ViewFlowControllerAsset defaultFlowAsset;


        
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
            if (!activeObject)
                return;
            
            var path = AssetDatabase.GetAssetPath(activeObject);
            path.GetDirectoryPath();

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
            settingsAsset.MarkDirty();
            
            view.settings = settingsAsset;
            view.gameObject.MarkDirty();
            
            AssetDatabase.Refresh();
        }

        #endregion
    }
}

