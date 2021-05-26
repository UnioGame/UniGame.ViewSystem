namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;
    using Runtime;
    using UniRx;

    public interface IViewStatus
    {
        IReadOnlyReactiveProperty<ViewStatus> Status { get; }

        IObservable<IView> OnHidden { get; }
        IObservable<IView> OnHiding { get; }
        IObservable<IView> OnShowing { get; }
        IObservable<IView> OnShown { get; }
        IObservable<IView> OnClosed { get; }
     
    }
}