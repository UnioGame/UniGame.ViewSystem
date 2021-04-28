using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;

namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;

    public interface IGameViewSystem : 
        IDisposable, 
        IViewLayoutProvider,
        IViewModelProvider
    {
        void CloseAll();
    }

}