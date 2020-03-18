namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;

    public class ViewsStackLayout : ViewLayout
    {
        private readonly Transform _root;
        
        private IView _activeView;
        private List<IView> _viewsOrder = new List<IView>(); 

        public ViewsStackLayout(Transform layout)
        {
            Layout = layout;
        }

        public IView ActiveView => _activeView;
        
        protected override void OnViewAdded<T>(T view)
        {
            if (_viewsOrder.Contains(view))
                return;
            
            view.OnShown.
                Where(x => x!=_activeView).
                Subscribe(ActivateView).
                AddTo(view.LifeTime);
            
            view.OnHidden.
                Where(x => x == _activeView).
                Subscribe(HideView).
                AddTo(view.LifeTime);
            
            ActivateView(view);
        }

        protected override void OnBeforeClose<T>(T view)
        {
            _viewsOrder.Remove(view);
            
            if (view == _activeView) {
                HideView(view);
            }
        }

        private void HideView(IView view)
        {
            //mark active view as empty
            _activeView = null;
            
            var lastView = _viewsOrder.LastOrDefault(x => x != view);
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
            _viewsOrder.Remove(view);
            _viewsOrder.Add(view);
            //show view if it inactive
            if(view.IsActive.Value == false)
                view.Show();
        }

    }
}