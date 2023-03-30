namespace UniGame.ViewSystem.Runtime
{
    using AddressableTools.Runtime;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using TMPro;
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
        
        public static bool SetValue(
            this Image target, 
            Color value)
        {
            if (target == null) return false;
            
            target.color = value;
            return true;
        }
        
        public static bool SetValue(
            this TextMeshProUGUI target, 
            string value)
        {
            if (target == null) return false;
            if (target.text == value) return false;
            target.text = value;
            return true;
        }
        
        public static bool SetValue(
            this Image target, 
            bool value)
        {
            if (target == null) return false;
            
            target.enabled = value;
            return true;
        }
    }
}