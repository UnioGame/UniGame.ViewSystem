namespace UniGame.UiSystem.Runtime
{
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniModules.UniGame.UiSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;

    public class SingleActiveScreenFlow : ViewFlowController
    {
        private readonly RecycleReactiveProperty<bool> _screenSuspended = new RecycleReactiveProperty<bool>();

        protected override void OnActivate(IViewLayoutContainer layouts)
        {
            var screenController = layouts.GetLayout(ViewType.Screen);
            var windowController = layouts.GetLayout(ViewType.Window);

            screenController.HasActiveView
                .Where(x => x)
                .Subscribe(x => windowController.CloseAll())
                .AddTo(windowController.LifeTime);

            windowController.HasActiveView
                .Where(x => x)
                .CombineLatest(windowController.OnBeginShow, (hasView, view) => view)
                .Where(x => x is IScreenSuspendingWindow)
                .Subscribe(x => _screenSuspended.Value = true)
                .AddTo(windowController.LifeTime);

            windowController.HasActiveView
                .Where(x => x)
                .CombineLatest(windowController.OnBeginShow, (hasView, view) => view)
                .Where(x => !(x is IScreenSuspendingWindow))
                .Subscribe(x => _screenSuspended.Value = false)
                .AddTo(windowController.LifeTime);

            windowController.HasActiveView
                .Where(x => !x)
                .Subscribe(x => _screenSuspended.Value = false)
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