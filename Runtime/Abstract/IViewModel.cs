namespace UniGame.ViewSystem.Runtime
{
    using System;
    using global::UniGame.Core.Runtime;
    using R3;


    public interface IViewModel : 
        IDisposable, ILifeTimeContext,
        ICloseableViewModel
    {
        
    }

    public interface ICloseableViewModel
    {
        ReactiveCommand CloseCommand { get; }
    }
}