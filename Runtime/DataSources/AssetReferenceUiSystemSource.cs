using System;

namespace UniModules.UniGame.UISystem.Runtime.DataSources
{
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class AssetReferenceUiSystemSource : 
        AssetReferenceT<ViewSystemSource>
    {
        public AssetReferenceUiSystemSource(string guid) : base(guid)
        {
        }
    }
}
