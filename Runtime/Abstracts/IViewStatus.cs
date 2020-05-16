namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;

    public interface IViewStatus
    {
        IObservable<IView> OnHidden { get; }
        IObservable<IView> OnShown { get; }
        IObservable<IView> OnClosed { get; }
    }
}