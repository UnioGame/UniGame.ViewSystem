namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class UiViewsSourceReference : AssetReferenceT<ViewsSettings>
    {
        public UiViewsSourceReference(string guid) : base(guid) { }
    }
}