namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class UiViewsSourceReference : AssetReferenceT<ViewsSource>
    {
        public UiViewsSourceReference(string guid) : base(guid) { }
    }
}