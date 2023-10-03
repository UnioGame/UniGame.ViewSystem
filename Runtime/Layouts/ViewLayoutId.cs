namespace UniGame.LeoEcs.ViewSystem.Layouts
{
    using System;
    using System.Collections.Generic;
    using Converters;
    using Sirenix.OdinInspector;
    using UiSystem.Runtime;
    using UnityEngine;
    
#if UNITY_EDITOR
    using UniModules.Editor;
    using UnityEditor;
#endif
    
    [Serializable]
    [ValueDropdown("@UniGame.LeoEcs.ViewSystem.Layouts.ViewLayoutId.GetLayoutsId()",IsUniqueList = true,DropdownTitle = "Equip Slot")]
    public struct ViewLayoutId
    {
        [SerializeField]
        private string value;

        #region statis editor data
        
        public static IEnumerable<ValueDropdownItem<ViewLayoutId>> GetLayoutsId()
        {
#if UNITY_EDITOR
            yield return new ValueDropdownItem<ViewLayoutId>()
            {
                Text = "NONE",
                Value = (ViewLayoutId)string.Empty,
            };
            
            foreach (var defaultType in GameViewSystem.DefaultTypes)
            {
                yield return new ValueDropdownItem<ViewLayoutId>()
                {
                    Text = defaultType,
                    Value = (ViewLayoutId)defaultType,
                };
            }
            
            var layouts = AssetEditorTools.GetAssets<ViewLayoutAsset>();
            
            foreach (var layout in layouts)
            {
                yield return new ValueDropdownItem<ViewLayoutId>()
                {
                    Text = layout.layoutId,
                    Value = (ViewLayoutId) layout.layoutId,
                };
            }
#endif
            yield break;
        }
                
        #endregion

        public static implicit operator string(ViewLayoutId v)
        {
            return v.value;
        }

        public static explicit operator ViewLayoutId(string v)
        {
            return new ViewLayoutId { value = v };
        }

        public override string ToString() => value.ToString();

        public override int GetHashCode() => string.IsNullOrEmpty(value) 
            ? 0 
            : value.GetHashCode();

        public ViewLayoutId FromInt(string data)
        {
            value = data;
            
            return this;
        }

        public override bool Equals(object obj)
        {
            if (obj is ViewLayoutId mask)
                return mask.value == value;
            
            return false;
        }

    }
}