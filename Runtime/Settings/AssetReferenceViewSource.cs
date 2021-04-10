namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class AssetReferenceViewSource : AssetReferenceT<ViewsSettings>
    {
        public AssetReferenceViewSource(string guid) : base(guid) { }
    }
}