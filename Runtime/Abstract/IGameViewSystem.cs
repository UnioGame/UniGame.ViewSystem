namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;

    public interface IGameViewSystem : 
        IDisposable, 
        IViewLayoutProvider
    {
        void CloseAll();
    }

}