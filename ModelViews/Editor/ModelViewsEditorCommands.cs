using System;
using UniGame.ModelViewsMap.Runtime.Settings;
using UniGame.UiSystem.Runtime;
using UniGame.Runtime.ReflectionUtils;
using UniModules.Editor;
using UniGame.ViewSystem.Runtime;
using UniModules.UniGame.ViewSystem;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.ViewSystem.ModelViews.Editor
{
    public static class ModelViewsEditorCommands
    {
        [MenuItem("UniGame/View System/Rebuild ModelsViewsSettings")]
        public static void Rebuild()
        {
            var settings = AssetEditorTools.GetAssets<ModelViewsModuleSettings>();
            
            foreach (var setting in settings) {
                Rebuild(setting);
                EditorUtility.SetDirty(setting);
            }
        }
        
                
        public static void Rebuild(ModelViewsModuleSettings settings)
        {
            settings.CleanUp();
            
            var baseViewType  = ViewSystemConstants.BaseViewType;
            var baseModelType = ViewSystemConstants.BaseModelType;
            
            var modelTypes = baseModelType.GetAssignableTypes();
            var typeArs    = new Type[1];
            
            //get all views
            foreach (var modelType in modelTypes) {
                typeArs[0] = modelType;
                var targetType = baseViewType.MakeGenericType(typeArs);
                var viewTypes  = targetType.GetAssignableTypes();
                settings.UpdateValue(modelType,viewTypes);
            }
            
            EditorUtility.SetDirty(settings);
        }

    }
}
