namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniRx;

    public class SingleActiveScreenFlow : ViewFlowController
    {
        private IView _activeView;

        private ReactiveProperty<bool> _screenSuspended = new ReactiveProperty<bool>(false);

        protected override void OnActivate(IViewLayoutContainer layouts)
        {
            var screenController = layouts.GetLayout(ViewType.Screen);
            var windowController = layouts.GetLayout(ViewType.Window);

            screenController.
                OnShown.
                Subscribe(x => {
                    UnityEngine.Debug.Log(x);
                    windowController.CloseAll();
                }).
                AddTo(windowController.LifeTime);

            windowController
                .OnShown
                .Subscribe(view => {
                    if (view is IScreenSuspendingWindow) {
                        _screenSuspended.Value = true;
                    }
                })
                .AddTo(windowController.LifeTime);

            windowController.OnClosed.Subscribe(view => {
                if (view is IScreenSuspendingWindow)
                    _screenSuspended.Value = false;
            });

            _screenSuspended
                .Skip(1)
                .ThrottleFrame(1)
                .DistinctUntilChanged()
                .Do(isSuspended =>
                {
                    if (isSuspended)
                    {
                        screenController.HideAll();
                    }
                    else
                    {
                        screenController.ShowLast();
                    }
                })
                .Subscribe()
                .AddTo(windowController.LifeTime);
        }
    }
}