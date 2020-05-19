using System;
using UniGreenModules.UniGame.AddressableTools.Runtime.AssetReferencies;

namespace UniModules.UniGame.UISystem.Runtime.DataSources
{
    [Serializable]
    public class AssetReferenceUiSystemSource : 
        DisposableAssetReference<UiSystemSource>
    {
        public AssetReferenceUiSystemSource(string guid) : base(guid)
        {
        }
    }
}
