namespace UniGame.ViewSystem.Runtime
{
    using System;
    using System.Runtime.CompilerServices;
    using AddressableTools.Runtime;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using TMPro;
    using UniModules.UniCore.Runtime.Utils;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.UI;

    public static class ValuesExtensions
    {
        public static async UniTask<bool> SetValue(
            this Image target, 
            AssetReferenceT<Sprite> value,
            ILifeTime lifeTime)
        {
            if (!target) return false;

            var sprite = value == null || !value.RuntimeKeyIsValid()
                ? null
                : await value.LoadAssetTaskAsync(lifeTime);
            
            target.enabled = sprite!=null;
            target.sprite = sprite;

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(this TextMeshProUGUI target, string value)
        {
            if (target == null) return false;
            if (target.text == value) return false;
            target.text = value;
            return true;
        }
        
        public static bool SetEnableValue(this Image target, bool value)
        {
            if (target == null) return false;
            
            target.enabled = value;
            return true;
        }
        
        
        public static bool SetValue(this TextMeshProUGUI text, string value, StringComparison comparison)
        {
            if (!text) return false;
            
            if (string.Equals(text.text, value,comparison))
                return false;

            text.text = value;
            return true;
        }

        
        public static bool SetValue(this TextMeshProUGUI text, Color color)
        {
            if (!text) return false;
            
            text.color = color;
            return true;
        }

        public static bool SetValue(this TextMeshProUGUI text, int value)
        {
            if (!text) return false;
            
            var stringValue = value.ToStringFromCache();
            return SetValue(text, stringValue);
        }

        public static bool SetValue(this Image target, bool enabled)
        {
            if (target == null) return false;
            target.enabled = enabled;
            return true;
        }
        
        public static bool SetValue(this Image target, Sprite value)
        {
            if (target == null || target.sprite == value) return false;
            
            var enabled = value != null;
            target.enabled = enabled;
            
            if(enabled) target.sprite = value;
            
            return true;
        }
        
        public static bool SetValue(this Image target, Color icon)
        {
            if (!target) return false;
            
            target.color = icon;

            return true;
        }
    }
}