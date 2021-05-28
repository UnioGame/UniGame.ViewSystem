namespace UniGame.UiSystem.Runtime
{
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniModules.UniGame.UiSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    using UnityEngine;

    public class SingleActiveScreenFlow : ViewFlowController
    {
        private int _openWindowCount;
        
        private int OpenWindowCount {
            get => _openWindowCount;
            set => _openWindowCount = value < 0 ? 0 : value;
        }

        private readonly RecycleReactiveProperty<bool> _screenSuspended = new RecycleReactiveProperty<bool>();

        protected override void OnActivate(IViewLayoutContainer layouts)
        {
            var screenController = layouts.GetLayout(ViewType.Screen);
            var windowController = layouts.GetLayout(ViewType.Window);

            screenController.
                OnShowing.
                Subscribe(x => windowController.CloseAll()).
                AddTo(windowController.LifeTime);

            windowController
                .OnShowing
                .Where(x => x is IScreenSuspendingWindow)
                .Subscribe(view => {
                    if (OpenWindowCount == 0)
                    {
                        _screenSuspended.Value = true;
                    }
                    OpenWindowCount++;
                })
                .AddTo(windowController.LifeTime);

            windowController.OnHidden
                .Where(x => x is IScreenSuspendingWindow)
                .Subscribe(view => OpenWindowCount--)
                .AddTo(LifeTime);

            windowController.OnClosed
                .Where(view => view is IScreenSuspendingWindow)
                .ThrottleFrame(1)
                .Subscribe(view => {
                    OpenWindowCount--;
                    if (OpenWindowCount == 0)
                    {
                        _screenSuspended.Value = false;
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