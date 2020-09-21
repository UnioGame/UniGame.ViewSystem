namespace UniGame.UiSystem.ModelViews.Runtime.Flow
{
    using System;
    using UiSystem.Runtime.Abstracts;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.UISystem.Runtime;
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