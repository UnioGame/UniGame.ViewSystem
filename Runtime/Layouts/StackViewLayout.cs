namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Linq;
    using Backgrounds.Abstract;
    using R3;
    using UniGame.Runtime.Rx.Extensions;
    using ViewSystem.Runtime;
     
    using UnityEngine;

    [Serializable]
    public class StackViewLayout : ViewLayout
    {
        #region inspector

        [Tooltip("same views will be reused if they are already in layout")]
        public bool applyIntents = true;

        #endregion
        
        private IBackgroundView _background;
        private IView _activeView;

        public IView LastView => Views.LastOrDefault(x => x != _activeView);
    
        public void Initialize(Transform layout, IBackgroundView background)
        {
            _background = background;
            
            Layout      = layout;

            OnClosed.When(x => x == _activeView, HideView)
                    .When(_ => _activeView == null, _ => UpdateActiveView())
                    .Subscribe()
                    .AddTo(LifeTime);

            OnBeginHide.Where(x => x == _activeView)
                       .Subscribe(HideView)
                       .AddTo(LifeTime);

            // OnIntent
            //     .Select(x => _activeView)
            //     .Subscribe(HideView)
            //     .AddTo(LifeTime);
            
            OnBeginShow.Where(x => x != _activeView)
                       .Subscribe(ActivateView)
                       .AddTo(LifeTime);
            
        }

        public override LayoutIntentResult Intent(string viewKey)
        {
            IView view = null;
            var result = new LayoutIntentResult() { stopPropagation = false };
            
            var isIntentActive = applyIntents && TryFindView(viewKey, out view) && view!=null;
            
            if (!isIntentActive) return result;
            if (view == ActiveView.CurrentValue) return result;
            if (view.Transform.parent != Layout) return result;
                
            view.Transform.SetAsLastSibling();

            result.view = view;
            result.stopPropagation = true;

            return result;
        }

        protected override bool IsAnyViewActive()
        {
            return _activeView != null && (_activeView.Status.CurrentValue == ViewStatus.Showing || 
                                           _activeView.Status.CurrentValue == ViewStatus.Shown) ||
                   LastView != null && (LastView.Status.CurrentValue == ViewStatus.Showing || 
                                        LastView.Status.CurrentValue == ViewStatus.Shown);
        }

        protected override void OnViewAdded<T>(T view)
        {
            if (view.IsVisible.CurrentValue == false)
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
                _background?.Hide();
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

            _background?.Show();
        }

        private bool TryFindView(string viewKey, out IView result )
        {
            result = null;
            
            if (string.IsNullOrEmpty(viewKey)) return false;

            foreach (var view in Views)
            {
                if(view.SourceName ==viewKey) {
                    result = view;
                    return true;
                }

                var viewType = view.GetType().Name;
                if (!viewKey.Equals(viewType, StringComparison.OrdinalIgnoreCase)) continue;
                
                result = view;
                return true;
            }

            return false;
        }
    }
}