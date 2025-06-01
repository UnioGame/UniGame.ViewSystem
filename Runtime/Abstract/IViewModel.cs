namespace UniGame.ViewSystem.Runtime
{
    using System;
    using global::UniGame.Core.Runtime;
    using UniRx;

    public interface IViewModel : 
        IDisposable, ILifeTimeContext,
        ICloseableViewModel
    {
        
    }

    public interface ICloseableViewModel
    {
        IReactiveCommand<Unit> CloseCommand { get; }
    }
}