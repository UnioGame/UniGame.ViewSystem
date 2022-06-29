namespace Taktika.UI.Common.Effects.Abstract
{
    using UnityEngine;

    public interface IPointToPointEffect : IEffect
    {
        void Initialize(Vector3 from, Vector3 to, string text = "");
    }
}