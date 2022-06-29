namespace Taktika.UI.Common.Effects.Factories
{
    using Abstract;
    using UnityEngine;
    using Taktika.UI.Common.Effects.Abstract;

    public class MonoUIPtPEffectFactory : IPointToPointEffectFactory
    {
        private readonly IPointToPointEffectFactory _effectFactory;
        
        public MonoUIPtPEffectFactory(IPointToPointEffectFactory effectFactory)
        {
            _effectFactory = effectFactory;
        }

        public IEffect Create(Vector3 position)
        {
            return _effectFactory.Create(position);
        }

        public IEffect Create(Vector3 from, Vector3 to, string text = "")
        {
            var effect = _effectFactory.Create(from, to, text);
            if (effect is MonoBehaviourEffect monoBehaviourEffect) {
                var effectRectTransform = monoBehaviourEffect.GetComponent<RectTransform>();
                if (effectRectTransform != null) {
                    effectRectTransform.position = from;
                }
            }
            
            return effect;
        }
    }
}