namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
#if ODIN_INSPECTOR
    [ValueDropdown("@UniGame.UISystem.Runtime.Utils.ViewSystemTool.GetViewIds()",IsUniqueList = true,DropdownTitle = "View Id")]
#endif
    public class ViewId
    {
        [SerializeField]
        public string value = string.Empty;

        public static implicit operator string(ViewId v)
        {
            return v.value;
        }

        public static explicit operator ViewId(string v)
        {
            return new ViewId { value = v };
        }

        public override string ToString() => value;

        public override int GetHashCode() => string.IsNullOrEmpty(value) 
            ? 0 
            : value.GetHashCode();

        public ViewId FromString(string data)
        {
            value = data;
            return this;
        }

        public override bool Equals(object obj)
        {
            if (obj is ViewId mask)
                return mask.value == value;
            
            return false;
        }
    }
}