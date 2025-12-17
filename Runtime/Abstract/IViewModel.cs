using UniGame.Core.Runtime;

namespace UniGame.ViewSystem.Runtime
{
    using System;
    using R3;
    
    public interface IViewModel : 
        IDisposable, 
        ILifeTimeContext,
        ICloseableViewModel
    {
        
    }

    public interface ICloseableViewModel
    {
        ReactiveCommand CloseCommand { get; }
    }
}