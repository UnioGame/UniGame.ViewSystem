namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniRx;

    public class SingleActiveViewFlowController : ViewFlowController
    {
        private IView _activeView;

        protected override void OnActivate(IViewLayoutContainer layouts)
        {
            var screenController = layouts.GetViewController(ViewType.Screen);
            var windowController = layouts.GetViewController(ViewType.Window);

            screenController.OnShown.
                Subscribe(x => windowController.CloseAll()).
                AddTo(windowController.LifeTime);
        }
        
    }
}