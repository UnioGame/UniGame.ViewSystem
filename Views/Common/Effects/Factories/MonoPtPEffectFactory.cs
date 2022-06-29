namespace Taktika.UI.Common.Effects.Factories
{
    using Abstract;
    using Effects.Abstract;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UnityEngine;

    public class MonoPtPEffectFactory : IPointToPointEffectFactory
    {
        private readonly MonoBehaviourEffect _effectPrefab;
        private readonly Transform _parent;
        
        public MonoPtPEffectFactory(MonoBehaviourEffect prefab, Transform parent)
        {
            _parent = parent;
            _effectPrefab = prefab;
        }
        
        public IEffect Create(Vector3 from)
        {
            return Create(from, from);
        }

        public IEffect Create(Vector3 from, Vector3 to, string text ="")
        {
            if (_effectPrefab == null)
                return null;
            
            var effect = _effectPrefab.SpawnActive(new Vector3(from.x, from.y, 0.0f), Quaternion.identity, _parent);
            if (effect is IPointToPointEffect pointToPointEffect) {
                pointToPointEffect.Initialize(from, to, text);
                return pointToPointEffect;
            }

            effect.Despawn();
            return null;
        }
    }
}