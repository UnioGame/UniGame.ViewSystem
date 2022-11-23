namespace UniGame.UI.Views
{
    using System.Collections;
    using UniGame.UiSystem.Runtime;
    using Core.Runtime;
    using ViewSystem.Runtime;
    using UnityEngine;

    public class UiScreen<TModel> : UiCanvasGroupView<TModel>
        where TModel : class, IViewModel
    {
        #region inspector
        
        [SerializeField]
        private string _hideAnimation = "Hide";
        
        [SerializeField]
        private string _showAnimation = "Show";
        
        [Header("screen fields")]
        [SerializeField] private Animation _animation;

        #endregion

        protected override IEnumerator OnShowProgressOverride(ILifeTime progress)
        {
            PlayAnimation(progress,_showAnimation);
            yield return WaitAnimation(_showAnimation);
        }
        
        protected override IEnumerator OnHidingProgressOverride(ILifeTime progress)
        {
            PlayAnimation(progress,_hideAnimation);
            yield return WaitAnimation(_hideAnimation);
        }

        private void PlayAnimation(ILifeTime progress,string id)
        {
            if (_animation == null)
                return;
            _animation.Play(id);
            progress.AddCleanUpAction(() => _animation.Stop());
        }

        private IEnumerator WaitAnimation(string animationName)
        {
            while (_animation && _animation.IsPlaying(animationName)) {
                yield return null;
            }
        }
    }
}