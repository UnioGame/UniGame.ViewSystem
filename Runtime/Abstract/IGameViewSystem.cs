using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;

namespace UniModules.UniGame.UISystem.Runtime.Abstract
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