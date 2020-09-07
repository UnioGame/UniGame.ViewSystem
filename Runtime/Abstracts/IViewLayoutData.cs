namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;

    public interface IViewLayoutData
    {
        IObservable<TView> ObserveView<TView>() where TView : IView;

        IObservable<IView> ViewCreated { get; }

    }
}