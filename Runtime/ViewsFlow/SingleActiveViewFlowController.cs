namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniRx;

    public class SingleActiveScreenFlow : ViewFlowController
    {
        private IView _activeView;

        protected override void OnActivate(IViewLayoutContainer layouts)
        {
            var screenController = layouts.GetLayout(ViewType.Screen);
            var windowController = layouts.GetLayout(ViewType.Window);

            screenController.
                OnShown.
                Subscribe(x => windowController.CloseAll()).
                AddTo(windowController.LifeTime);
        }
        
    }
}