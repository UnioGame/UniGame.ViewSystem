using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;

namespace UniGame.ViewSystem.Runtime
{
    using System;

    public interface IGameViewSystem : 
        IDisposable, 
        IViewLayoutProvider,
        IViewModelResolver
    {
        void CloseAll();
    }

}