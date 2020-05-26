namespace UniGame.UiSystem.Runtime
{
    using System;

    public interface IGameViewSystem : 
        IDisposable, 
        IViewLayoutProvider
    {
        void CloseAll();
    }

}