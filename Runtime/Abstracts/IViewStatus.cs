namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniModules.UniGame.UISystem.Runtime;
    using UniRx;

    public interface IViewStatus
    {
        IReadOnlyReactiveProperty<ViewStatus> Status { get; }

        IObservable<IView> OnHidden { get; }
        IObservable<IView> OnShown { get; }
        IObservable<IView> OnClosed { get; }
    }
}