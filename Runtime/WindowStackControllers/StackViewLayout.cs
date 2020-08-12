namespace UniGame.UiSystem.Runtime
{
    using System.Linq;
    using Abstracts;
    using Backgrounds.Abstract;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;

    public class StackViewLayout : ViewLayout
    {
        private readonly IBackgroundView _background;

        private IView _activeView;

        public StackViewLayout(Transform layout, IBackgroundView background)
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

        protected override void OnViewAdded<T>(T view) => ActivateView(view);

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
            if (lastView == null) {
                _background?.Hide();
                return;
            }
            
            ActivateView(lastView);
        }

        private void ActivateView(IView view)
        {
            var previous = _activeView;
            _activeView = view;
            
            previous?.Hide();
            
            //update top of stack
            Remove(view);
            Add(view);
            //show view if it inactive
            if(view.IsActive.Value == false)
                view.Show();

            _background?.Show();
        }
    }
}