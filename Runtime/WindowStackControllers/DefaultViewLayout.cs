namespace UniGame.UiSystem.Runtime
{
    using System.Linq;
    using Abstracts;
    using Backgrounds.Abstract;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.UISystem.Runtime;
    using UniRx;
    using UnityEngine;

    public class DefaultViewLayout : ViewLayout
    {
        private readonly IBackgroundView _background;
        
        private IView _activeView;

        public DefaultViewLayout(Transform layout, IBackgroundView background)
        {
            _background = background;
            Layout = layout;
            
            OnClosed.Where(x => x == _activeView).
                Subscribe(HideView).
                AddTo(LifeTime);
            
            OnHidden.Where(x => x == _activeView).
                Subscribe(HideView).
                AddTo(LifeTime);
            
            OnShown.Where(x => x!=_activeView).
                Subscribe(ActivateView).
                AddTo(LifeTime);
        }

        protected override void OnViewAdded<T>(T view)
        {
            ActivateView(view);
        }

        protected override void OnBeforeClose(IView view)
        {
            if (view == _activeView) {
                HideView(view);
            }
        }

        private void HideView(IView view)
        {
            //mark active view as empty
            _activeView = null;
            
            var lastView = Views.LastOrDefault(x => x != view);
            //empty view stack or only active
            if (lastView == null && _background != null && _background.Status.Value == ViewStatus.Shown) {
                _background.Hide();
            }
        }

        private void ActivateView(IView view)
        {
            _activeView = view;

            //update top of stack
            Remove(view);
            Add(view);
            //show view if it inactive
            if(view.IsActive.Value == false)
                view.Show();

            if(_background != null && _background.Status.Value != ViewStatus.Shown) {
                _background.Show();
            }
        }
    }
}