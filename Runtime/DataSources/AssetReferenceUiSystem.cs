namespace UniGame.ModelViewsMap.Runtime.Settings
{
    using System;
    using UiSystem.Runtime;
    using UniModules.UniGame.AddressableTools.Runtime.AssetReferencies;

    [Serializable]
    public class AssetReferenceUiSystem : AssetReferenceComponent<GameViewSystemAsset>
    {
        public AssetReferenceUiSystem(string guid) : base(guid)
        {
        }
    }
}