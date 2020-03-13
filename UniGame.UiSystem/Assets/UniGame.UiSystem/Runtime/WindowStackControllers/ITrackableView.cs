namespace UniGame.UiSystem.Runtime
{
    using System;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;

    public interface ITrackableView : IView
    {
        IObservable<ITrackableView> OnHide   { get; }
        IObservable<ITrackableView> OnShow   { get; }
        IObservable<ITrackableView> OnClosed { get; }
    }
}