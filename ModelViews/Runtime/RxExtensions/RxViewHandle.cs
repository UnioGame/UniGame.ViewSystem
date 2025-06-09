namespace UniGame.UiSystem.ModelViews.Runtime.RxExtensions
{
    using System;
    using UiSystem.Runtime;
    using ViewSystem.Runtime;

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
