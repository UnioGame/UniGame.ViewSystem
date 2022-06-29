namespace Taktika.UI.Common.Effects.Factories.Abstract
{
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UnityEngine;
    using Taktika.UI.Common.Effects.Abstract;

    public interface IEffectFactory : IFactory<Vector3, IEffect>
    {
    }
}