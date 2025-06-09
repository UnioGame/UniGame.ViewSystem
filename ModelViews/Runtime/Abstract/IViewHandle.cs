namespace UniGame.UiSystem.Runtime
{
    using System;
    using Core.Runtime;
    using R3;
    using ViewSystem.Runtime;
     

    public interface IViewHandle : 
        IObservable<IViewHandle>, 
        ILifeTimeContext,
        IViewCommands,
        IDisposable
    {
        ReadOnlyReactiveProperty<ViewStatus> Status { get; }

        IView View { get; }

        IViewModel Model { get; }
        
        Type       ModelType { get; }

        Type ViewType { get; }

        void SetView(IView view);
    }
}