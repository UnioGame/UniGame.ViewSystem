namespace UniGreenModules.UniGame.UiSystem.Runtime.Settings
{
    using System;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class UiViewsSourceReference : AssetReferenceT<UiViewsSource>
    {
        public UiViewsSourceReference(string guid) : base(guid) { }
    }
}