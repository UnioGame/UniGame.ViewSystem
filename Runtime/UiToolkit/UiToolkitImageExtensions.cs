namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using Cysharp.Threading.Tasks;
    using UniGame.AddressableTools.Runtime;
    using UniGame.Core.Runtime;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.UIElements;

    public static class UiToolkitImageExtensions
    {
        public static void SetSprite(this Image image, Sprite sprite)
        {
            if (image == null)
                return;

            image.sprite = sprite;
            image.style.display = sprite == null
                ? DisplayStyle.None
                : DisplayStyle.Flex;
        }

        public static async UniTask<bool> SetSpriteAsync(
            this Image image,
            AssetReferenceSprite reference,
            ILifeTime lifeTime)
        {
            if (image == null || lifeTime == null || lifeTime.IsTerminated)
                return false;

            image.SetSprite(null);

            if (reference == null || !reference.RuntimeKeyIsValid())
                return false;

            var (isCanceled, sprite) = await reference
                .LoadAssetTaskAsync(lifeTime)
                .AttachExternalCancellation(lifeTime.Token)
                .SuppressCancellationThrow();

            if (isCanceled || lifeTime.IsTerminated)
                return false;

            image.SetSprite(sprite);
            return sprite;
        }
    }
}