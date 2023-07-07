namespace UniGame.UI.Views
{
    using UniGame.UiSystem.Runtime;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
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

        protected override async UniTask OnShowProgressOverride(ILifeTime progress)
        {
            PlayAnimation(progress,_showAnimation);
            await WaitAnimation(_showAnimation);
        }
        
        protected override async UniTask OnHidingProgressOverride(ILifeTime progress)
        {
            PlayAnimation(progress,_hideAnimation);
            await WaitAnimation(_hideAnimation);
        }

        private void PlayAnimation(ILifeTime progress,string id)
        {
            if (_animation == null)
                return;
            _animation.Play(id);
            progress.AddCleanUpAction(() => _animation.Stop());
        }

        private async UniTask WaitAnimation(string animationName)
        {
            while (_animation && _animation.IsPlaying(animationName))
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }
    }
}