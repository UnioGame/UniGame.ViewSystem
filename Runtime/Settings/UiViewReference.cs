using System.Collections.Generic;
using UniModules.UniGame.ViewSystem.Editor.UiEditor;

namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UniModules.UniGame.Core.Runtime.SerializableType;
    using UniModules.UniGame.Core.Runtime.SerializableType.Attributes;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class UiViewReference
#if ODIN_INSPECTOR
        : Sirenix.OdinInspector.ISearchFilterable
#endif
    {
        public string AssetGUID = string.Empty;

        [Space(2)] public AssetReferenceGameObject View;

        [Space(2)] public string ViewName;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.GUIColor(g: 1.0f, r: 1.0f, b: 0.5f)]
#endif
        [Space(2)]
        public string Tag = string.Empty;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [STypeFilter(typeof(IView))]
        public SType Type;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ValueDropdown(nameof(GetModelValue))]
#endif
        public SType ModelType;

        //type of view model
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ValueDropdown(nameof(GetModelDropdowns))]
#endif
        [Space(2)]
        public SType ViewModelType;

        public int PoolingPreloadCount = 1;
        
        private string GetModelTypeName()
        {
            return $"ModelType : {ModelType?.Type?.GetFormattedName()}";
        }

#if ODIN_INSPECTOR

        private IEnumerable<Sirenix.OdinInspector.ValueDropdownItem<SType>> GetModelValue()
        {
            yield return new Sirenix.OdinInspector.ValueDropdownItem<SType>()
            {
                Text = ModelType.Type == null 
                    ? "(empty)" 
                    : ModelType.Type.Name,
                Value = ModelType
            };
        }
        
        private IEnumerable<Sirenix.OdinInspector.ValueDropdownItem<SType>> GetModelDropdowns()
        {
            var type = ModelType.Type;
            
            if (type == null || (!type.IsAbstract && !type.IsInterface))
            {
                yield return new Sirenix.OdinInspector.ValueDropdownItem<SType>()
                {
                    Text = ModelType?.Type.Name,
                    Value = ModelType
                };
                yield break;
            }

            var items = ViewModelsAssemblyMap.GetValue(ModelType);
            foreach (var item in items)
            {
                yield return new Sirenix.OdinInspector.ValueDropdownItem<SType>()
                {
                    Text = item.Type.Name,
                    Value = item
                };
            }
        }
        
#endif
        public bool IsMatch(string searchString)
        {
            var isMatch = AssetGUID.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;

#if UNITY_EDITOR
            var gameObject = View.editorAsset;
            if (gameObject)
            {
                var gameObjectName = gameObject.name;
                isMatch |= gameObjectName.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            
#endif
            isMatch |= ViewName.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
            isMatch |= Tag.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
            isMatch |= Type.TypeName?.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
            isMatch |= ModelType.TypeName?.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
            isMatch |= ViewModelType.TypeName?.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;

            return isMatch;
        }
    }
    
#if !ODIN_INSPECTOR
    public interface ISearchFilterable
    {
        
    }
#endif
}