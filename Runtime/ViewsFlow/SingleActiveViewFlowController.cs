namespace UniGame.UiSystem.Runtime
{
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.UiSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;

    public class SingleActiveScreenFlow : ViewFlowController
    {
        private int _openWindowCount;
        
        private int OpenWindowCount {
            get => _openWindowCount;
            set {
                _openWindowCount = value;
                if (_openWindowCount < 0)
                    _openWindowCount = 0;
            }
        }

        private readonly ReactiveProperty<bool> _screenSuspended = new ReactiveProperty<bool>(false);

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
                        if(OpenWindowCount == 0) {
                            _screenSuspended.Value = true;
                        }
                        OpenWindowCount++;
                    }
                })
                .AddTo(windowController.LifeTime);

            windowController.OnHidden.Subscribe(view => {
                if (view is IScreenSuspendingWindow) {
                    OpenWindowCount--;
                }
            }).AddTo(LifeTime);

            windowController.OnClosed.Subscribe(view => {
                if (view is IScreenSuspendingWindow) {
                    OpenWindowCount--;
                    if (OpenWindowCount == 0) {
                        _screenSuspended.Value = false;
                    }
                }
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