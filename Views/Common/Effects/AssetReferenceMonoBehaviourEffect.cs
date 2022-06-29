namespace Taktika.UI.Common.Effects
{
    using System;
    using UniModules.UniGame.AddressableTools.Runtime.AssetReferencies;
    using Taktika.UI.Common.Effects.Abstract;

    [Serializable]
    public class AssetReferenceMonoBehaviourEffect : AssetReferenceComponent<MonoBehaviourEffect>
    {
        public AssetReferenceMonoBehaviourEffect(string guid) : base(guid)
        {
        }
    }
}