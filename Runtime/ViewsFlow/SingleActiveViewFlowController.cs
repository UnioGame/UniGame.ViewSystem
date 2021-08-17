namespace UniGame.UiSystem.Runtime
{
    using System;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniModules.UniGame.UiSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;

    [Serializable]
    public class SingleActiveScreenFlow : ViewFlowController
    {
        private static Type suspendType = typeof(IScreenSuspendingWindow);
        private readonly RecycleReactiveProperty<bool> _screenSuspended = new RecycleReactiveProperty<bool>();

        protected override void OnActivate(IViewLayoutContainer layouts)
        {
            var screenController = layouts.GetLayout(ViewType.Screen);
            var windowController = layouts.GetLayout(ViewType.Window);

            screenController.ActiveView
                .Where(v => v != null)
                .Do(_ => windowController.CloseAll())
                .Subscribe()
                .AddTo(windowController.LifeTime);

            windowController.ActiveView
                .Where(x => x != null)
                .CombineLatest(windowController.OnBeginShow, (hasView, view) => view)
                .When(x => x is IScreenSuspendingWindow, x => _screenSuspended.Value = true,
                    x => _screenSuspended.Value = false)
                .Subscribe()
                .AddTo(windowController.LifeTime);

            windowController.ActiveView
                .Where(v=>v == null)
                .Do(_ => _screenSuspended.Value = false)
                .Subscribe()
                .AddTo(windowController.LifeTime);

            _screenSuspended
                .Skip(1)
                .WhenTrue(x => screenController.Suspend())
                .WhenFalse(x => screenController.Resume())
                .Subscribe()
                .AddTo(windowController.LifeTime);
        }
    }
}