using System.Collections.Generic;
using UniGame.AddressableTools.Runtime;
using UniModules.UniGame.ViewSystem;

namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using Core.Runtime.SerializableType;
    using Core.Runtime.SerializableType.Attributes;
    using ViewSystem.Runtime;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public class UiViewReference : IEquatable<UiViewReference>
#if ODIN_INSPECTOR
        ,ISearchFilterable
#endif
    {
        public string AssetGUID = string.Empty;

#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        [Space(2)] public AssetReferenceGameObject View;

        [Space(2)] public string ViewName;

#if ODIN_INSPECTOR
        [GUIColor(g: 1.0f, r: 1.0f, b: 0.5f)]
#endif
        [Space(2)]
        public string Tag = string.Empty;

#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        [STypeFilter(typeof(IView))]
        public SType Type;

#if ODIN_INSPECTOR
        [ValueDropdown(nameof(GetModelValue))]
#endif
        public SType ModelType;

        //type of view model
#if ODIN_INSPECTOR
        [ValueDropdown(nameof(GetModelDropdowns))]
#endif
        [Space(2)]
        public SType ViewModelType;

        public int PoolingPreloadCount = 1;

        public List<AssetReferenceSpriteAtlas> Atlases = new List<AssetReferenceSpriteAtlas>();

        public int Hash;
        
        public override int GetHashCode()
        {
            var hash = HashCode.Combine(AssetGUID.GetHashCode(), 
                ViewModelType.GetHashCode(), 
                Tag.GetHashCode(),
                ModelType.GetHashCode());
            
            return hash;
        }

        public bool Equals(UiViewReference other)
        {
            return other!=null && other.GetHashCode() == GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is UiViewReference other && Equals(other);
        }

        private string GetModelTypeName()
        {
            return $"ModelType : {ModelType.Type?.GetFormattedName()}";
        }

#if ODIN_INSPECTOR

        public IEnumerable<ValueDropdownItem<SType>> GetModelValue()
        {
            yield return new ValueDropdownItem<SType>()
            {
                Text = ModelType.Type == null 
                    ? "(empty)" 
                    : ModelType.Type.Name,
                Value = ModelType
            };
        }
        
        private IEnumerable<ValueDropdownItem<SType>> GetModelDropdowns()
        {
            return ViewSystemUtils.GetModelSTypeVariants(ModelType.Type);
        }
        
#endif
        
        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            
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