namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;

    // Хороший интерфейс, дейстивтельно подходит и в Layout и в IView
    public interface IViewStatus
    {
        IObservable<IView> OnHidden { get; }
        IObservable<IView> OnShown { get; }
        IObservable<IView> OnClosed { get; }
    }
}