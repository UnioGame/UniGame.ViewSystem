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
        
        public static bool SetEnableValue(this Image target, bool value)
        {
            if (target == null) return false;
            
            target.enabled = value;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(this TextMeshProUGUI text, string value)
        {
            if (text == null || 
                value == null) return false;
            
            if (string.Equals(text.text, value)) return false;

            text.text = value;
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(this TextMeshPro text, string value)
        {
            if (text == null || value == null) return false;
            
            if (string.Equals(text.text, value)) return false;

            text.text = value;
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(this TMP_InputField text, string value)
        {
            if (text == null || value == null) return false;
            
            if (string.Equals(text.text, value)) return false;

            text.text = value;
            
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
        
        public static bool SetValue(this TextMeshPro text, Color color)
        {
            if (!text) return false;
            text.color = color;
            return true;
        }
        
        public static bool SetValue(this TMP_InputField text, Color color)
        {
            if (!text && text.textComponent!=null) return false;
            text.textComponent.color = color;
            return true;
        }
        
        public static bool SetValue(this Button button, Color color)
        {
            if(button == null || button.image == null) return false;
            button.image.color = color;
            return true;
        }

        public static bool SetValue(this TextMeshProUGUI text, int value)
        {
            if (!text) return false;
            
            var stringValue = value.ToStringFromCache();
            return SetValue(text, stringValue);
        }
        
        public static bool SetValue(this Image target, UniTask<Sprite> value)
        {
            SetValueAsync(target, value).Forget();
            return target!=null;
        }
        
        public static async UniTask<bool> SetValueAsync(this Image target, UniTask<Sprite> sprite)
        {
            if (target == null) return false;
            var value = await sprite;
            return SetValue(target, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(this Image target, bool enabled)
        {
            if (target == null) return false;
            target.enabled = enabled;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(this Image target, Sprite value)
        {
            if (target == null) return false;
            
            var enabled = value != null;
            target.enabled = enabled;
            
            if(enabled) target.sprite = value;
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(this RawImage target, Sprite value)
        {
            if (target == null) return false;
            
            var enabled = value != null;
            target.enabled = enabled;
            
            if(enabled) target.texture = value.texture;
            return true;
        }
        
        public static bool SetValue(this RawImage target, Color icon)
        {
            if (!target) return false;
            target.color = icon;
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