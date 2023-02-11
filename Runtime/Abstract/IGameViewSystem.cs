using UniGame.ViewSystem.Runtime;

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