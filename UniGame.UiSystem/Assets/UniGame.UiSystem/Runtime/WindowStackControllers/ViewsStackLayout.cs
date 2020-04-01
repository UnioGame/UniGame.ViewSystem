namespace UniGame.UiSystem.Runtime
{
    using System.Linq;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniUiSystem.Runtime.Utils;
    using UniRx;
    using UnityEngine;

    public class ViewsStackLayout : ViewLayout
    {
        private readonly CanvasGroup _background;

        private IView _activeView;

        public ViewsStackLayout(Transform layout,CanvasGroup background)
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
            _background?.SetState(0, false, false);
            
            //mark active view as empty
            _activeView = null;
            
            var lastView = Views.LastOrDefault(x => x != view);
            //empty view stack or only active
            if (lastView == null) return;
            
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
            
            _background?.SetState(1);
        }

    }
}