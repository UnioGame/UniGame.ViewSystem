namespace UniGame.ViewSystem.Runtime
{
    using AddressableTools.Runtime;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
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
    }
}