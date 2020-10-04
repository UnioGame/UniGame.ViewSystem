namespace UniGame.UiSystem.ModelViews.Runtime.RxExtensions
{
    using System;
    using Flow;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime;
    using UniRx;

    public static class RxViewHandle 
    {
        public static IObservable<IViewHandle> WhenStatus(
            this IObservable<IViewHandle> handle,
            ViewStatus status)
        {
            return null;
        }
    }
}
