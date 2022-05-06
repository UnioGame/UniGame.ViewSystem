namespace UniGame.UiSystem.Runtime
{
    using System.Linq;
    using Backgrounds.Abstract;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.UISystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    using UnityEngine;

    public class StackViewLayout : ViewLayout
    {
        private readonly IBackgroundView _background;

        private IView _activeView;

        public IView LastView => Views.LastOrDefault(x => x != _activeView);

        public StackViewLayout(Transform layout, IBackgroundView background)
        {
            _background = background;
            Layout      = layout;

            OnClosed.When(x => x == _activeView, HideView)
                    .When(_ => _activeView == null, _ => UpdateActiveView())
                    .RxSubscribe()
                    .AddTo(LifeTime);

            OnBeginHide.Where(x => x == _activeView)
                       .RxSubscribe(HideView)
                       .AddTo(LifeTime);

            // OnIntent
            //     .Select(x => _activeView)
            //     .Subscribe(HideView)
            //     .AddTo(LifeTime);
            
            OnBeginShow.Where(x => x != _activeView)
                       .RxSubscribe(ActivateView)
                       .AddTo(LifeTime);
            
        }

        protected override bool IsAnyViewActive()
        {
            return _activeView != null && (_activeView.Status.Value == ViewStatus.Showing || _activeView.Status.Value == ViewStatus.Shown) ||
                   LastView != null && (LastView.Status.Value == ViewStatus.Showing || LastView.Status.Value == ViewStatus.Shown);
        }

        protected override void OnViewAdded<T>(T view)
        {
            if (view.IsVisible.Value == false)
            {
                view.Show();
            }
            else
            {
                ActivateView(view);
            }
        }

        protected override void OnBeforeClose(IView view)
        {
            if (view == _activeView) {
                HideView(view);
            }
        }

        private void UpdateTop(IView view)
        {
            Remove(view);
            Add(view);
        }

        private void UpdateActiveView()
        {
            if(Views.Count == 0)
                return;

            ActivateView(Views.Last());
        }

        private void HideView(IView view)
        {
            _activeView = null;
            
            var lastView = Views.LastOrDefault(x => x != view);
            if (lastView == null) 
            {
                if (_background != null) // can be UnityEngine.Object
                    _background.Hide();
                
                return;
            }

            UpdateTop(lastView);
            ShowLast();
        }

        private void ActivateView(IView view)
        {
            var previous = _activeView;
            _activeView = view;
            
            previous?.Hide();

            UpdateTop(view);

            if (_background != null) // can be UnityEngine.Object
                _background.Show();
        }
    }
}