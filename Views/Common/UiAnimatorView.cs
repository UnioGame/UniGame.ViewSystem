namespace UniGame.UI.Views
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    using UniGame.Routines.Runtime.Extension;
    using UniGame.UiSystem.Runtime;
    using Core.Runtime;
    using ViewSystem.Runtime;
    using UniModules.UniUiSystem.Runtime.Utils;
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public abstract class UiAnimatorView<TWindowModel> : UiCanvasGroupView<TWindowModel> where TWindowModel : class, IViewModel
    {
#if ODIN_INSPECTOR
        [FoldoutGroup(nameof(Animator), false)]
#endif
        [SerializeField]
        private string _showStateName = "Show";
#if ODIN_INSPECTOR
        [FoldoutGroup(nameof(Animator), false)]
#endif
        [SerializeField]
        private string _hideStateName = "Hide";
        
#if ODIN_INSPECTOR
        [Required]
        [FoldoutGroup(nameof(Animator), false)]
#endif
        [SerializeField]
        protected Animator _animator;
        
#if ODIN_INSPECTOR
        [Required]
        [FoldoutGroup(nameof(Animator), false)]
#endif
        [SerializeField]
        protected List<ViewBase> _nestedAnimatedViews;
        
        private int _showStateHash;
        private int _hideStateHash;

        protected override void OnAwake()
        {
            base.OnAwake();

            _animator = _animator == null 
                ? GetComponent<Animator>() 
                : _animator;
            
            _animator.enabled = false;
            _showStateHash = Animator.StringToHash(_showStateName);
            _hideStateHash = Animator.StringToHash(_hideStateName);
        }
        
#if UNITY_EDITOR
        protected override void OnViewValidate()
        {
            base.OnViewValidate();
            _animator = _animator == null ? GetComponent<Animator>() : _animator;
        }
#endif

        // костыль для фикса проблемы с мигающими вьюшками
        // вложенные корутины ждут слишком много кадров, из-за чего вьюшка успевает показаться до того, как начнется анимация
        // срабатывает VisibilityHandler, устанавливает альфу = 1
        // параллельно выполняется OnShowProgress, но когда доходит до yield return, не заходит в OnShowProgressOverride до конца кадра
        protected override async UniTask OnViewInitialize(TWindowModel model)
        {
            await base.OnViewInitialize(model);
            
            if (_animator != null && _animator.HasState(0, _showStateHash)) {
                VisibilityHandler?.Dispose();
            }
        }

        protected override IEnumerator OnShowProgressOverride(ILifeTime progressLifeTime)
        {
            CanvasGroup.SetState(visibleState);
            
            if (_nestedAnimatedViews.Any())
            {
                foreach (var nestedView in _nestedAnimatedViews)
                {
                    nestedView.Show();
                }
            }
            
            _animator.enabled = true;
            yield return _animator.WaitStateEnd(_showStateHash);
        }

        protected override IEnumerator OnCloseProgressOverride(ILifeTime progressLifeTime)
        {
            foreach (var nestedView in _nestedAnimatedViews)
            {
                nestedView.Close();
            }
            
            yield return _animator.WaitStateEnd(_hideStateHash);
        }

        protected override IEnumerator OnHidingProgressOverride(ILifeTime progressLifeTime)
        {
            foreach (var nestedView in _nestedAnimatedViews)
            {
                nestedView.Hide();
            }
            
            yield return _animator.WaitStateEnd(_hideStateHash);
            _animator.enabled = false;
        }
    }
}