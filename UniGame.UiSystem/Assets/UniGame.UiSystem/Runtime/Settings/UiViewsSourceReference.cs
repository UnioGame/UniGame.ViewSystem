namespace UniGreenModules.UniGame.UiSystem.Runtime.Settings
{
    using System;
    using AddressableTools.Runtime.AssetReferencies;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class UiViewsSourceReference : DisposableAssetReference<UiViewsSource>
    {
        public UiViewsSourceReference(string guid) : base(guid) { }
    }
}