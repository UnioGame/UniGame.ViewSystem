namespace Taktika.UI.Common.Effects.Abstract
{
    using System;
    using System.Collections;
    using DG.Tweening;
    using Sirenix.OdinInspector;
    using UniGame.DOTweenRx.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniRoutine.Runtime;
    using UniModules.UniRoutine.Runtime.Extension;
    using UniModules.UniGame.DoTweenRoutines.Runtime;
    using UniRx;
    using UnityEngine;

    public abstract class MonoBehaviourEffect : MonoBehaviour, IEffect
    {
        [Space]
        [SerializeField]
        [FoldoutGroup("Parameters")]
        private float _delayBeforeDestroy = 1.0f;
        
        protected Sequence AnimationSequence;

        public virtual IObservable<Sequence> Play()
        {
            DoTweenExtension.KillSequence(ref AnimationSequence);
            AnimationSequence = CreateSequence();
            
            return AnimationSequence.PlayAsObservable().DoOnCompleted(OnEnd);
        }

        protected virtual Sequence CreateSequence()
        {
            return DOTween.Sequence();
        }

        protected virtual void OnEnd()
        {
            DespawnEffect().
                Execute().
                AsDisposable().
                AddTo(this);
        }

        private IEnumerator DespawnEffect()
        {
            yield return this.WaitForSeconds(_delayBeforeDestroy);
            
            this.Despawn();
        }
    }
}