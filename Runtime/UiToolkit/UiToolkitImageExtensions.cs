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

            var isLoaded = reference.OperationHandle.IsValid();
            var handle = isLoaded
                ? reference.OperationHandle.Convert<Sprite>()
                : reference.LoadAssetAsync();

            handle.AddTo(lifeTime, incrementRefCount: isLoaded);

            var isCanceled = await handle
                .ToUniTask(cancellationToken: lifeTime.Token)
                .SuppressCancellationThrow();

            if (isCanceled || lifeTime.IsTerminated)
                return false;

            var sprite = handle.Result;
            image.SetSprite(sprite);
            return sprite != null;
        }
    }
}