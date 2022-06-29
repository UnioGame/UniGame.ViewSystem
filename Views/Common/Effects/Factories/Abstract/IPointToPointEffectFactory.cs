namespace Taktika.UI.Common.Effects.Factories.Abstract
{
    using UnityEngine;
    using Taktika.UI.Common.Effects.Abstract;

    public interface IPointToPointEffectFactory : IEffectFactory
    {
        IEffect Create(Vector3 from, Vector3 to, string text = "");
    }
}