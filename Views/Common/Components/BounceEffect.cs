namespace Taktika.UI.Components
{
    using DG.Tweening;
    using UniModules.UniGame.DoTweenRoutines.Runtime;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class BounceEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Header("Shrink")]
        
        [SerializeField] private float _shrinkDuration = .12f;
        [SerializeField] private Vector3 _shrinkScale = new Vector3(.9f, .9f, .9f);
        [SerializeField] private Ease _shrinkEase = Ease.InOutQuad;
        
        [Header("Bounce Back")]
        
        [SerializeField] private float _bounceBackDuration = 1f;
        [SerializeField] private Ease _bounceBackEase = Ease.OutElastic;
        
        private Sequence _shrinkSequence;
        private Sequence _bounceSequence;

        public void OnPointerClick(PointerEventData eventData) { }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Shrink();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            BounceBack();
        }
        
        private void Shrink()
        {
            DoTweenExtension.KillSequence(ref _shrinkSequence);
            DoTweenExtension.KillSequence(ref _bounceSequence);

            _shrinkSequence = DOTween.Sequence();
            var shrinkTween = transform.DOScale(_shrinkScale, _shrinkDuration)
                .SetEase(_shrinkEase);

            _shrinkSequence.Append(shrinkTween)
                .Play();
        }

        private void BounceBack()
        {
            DoTweenExtension.KillSequence(ref _bounceSequence);
            DoTweenExtension.KillSequence(ref _shrinkSequence);

            _bounceSequence = DOTween.Sequence();
            var bounceTween = transform.DOScale(Vector3.one, _bounceBackDuration)
                .SetEase(_bounceBackEase);

            _bounceSequence.Append(bounceTween)
                .Play();
        }

        private void OnDestroy()
        {
            DoTweenExtension.KillSequence(ref _shrinkSequence);
            DoTweenExtension.KillSequence(ref _bounceSequence);
        }
    }
}