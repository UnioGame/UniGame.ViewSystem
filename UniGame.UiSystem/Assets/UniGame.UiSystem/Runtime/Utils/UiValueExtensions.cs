namespace UniGreenModules.UniUiSystem.Runtime.Utils
{
    using System;
    using TMPro;
    using UniCore.Runtime.Utils;
    using UnityEngine;
    using UnityEngine.UI;

    public static class UiValueExtensions
    {

        public static bool SetValue(this TextMeshProUGUI text, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (!text) return false;
            
            if (string.Equals(text.text, value,comparison))
                return false;

            text.text = value;
            return true;
        }
        
        
        public static bool SetValue(this TextMeshProUGUI text, int value)
        {
            if (!text) return false;
            
            var stringValue = value.ToStringFromCache();
            if (string.Equals(text.text, stringValue,StringComparison.OrdinalIgnoreCase))
                return false;

            text.text = stringValue;
            return true;
        }

        public static bool SetValue(this Image target, Sprite icon)
        {
            if (!target) return false;
            
            target.sprite = icon;

            return true;
        }
        
    }
}
