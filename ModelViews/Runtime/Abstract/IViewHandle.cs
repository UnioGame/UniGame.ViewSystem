namespace UniGame.UiSystem.ModelViews.Runtime.Flow
{
    using System;
    using Core.Runtime;
    using UniModules.UniGame.UISystem.Runtime;
    using ViewSystem.Runtime;
    using UniRx;

    public interface IViewHandle : 
        IObservable<IViewHandle>, 
        ILifeTimeContext,
        IViewCommands,
        IDisposable
    {
        IReadOnlyReactiveProperty<ViewStatus> Status { get; }

        IView View { get; }

        IViewModel Model { get; }
        
        Type       ModelType { get; }

        Type ViewType { get; }

        void SetView(IView view);
    }
}