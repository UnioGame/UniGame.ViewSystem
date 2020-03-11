namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using System;
    using UniCore.Runtime.Interfaces;
    
    public interface IGameViewSystem : ILifeTimeContext, 
        IViewElementFactory, 
        IDisposable
    {
    }
}