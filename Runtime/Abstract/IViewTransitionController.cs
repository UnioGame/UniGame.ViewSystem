namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;
    using Core.Runtime.Interfaces;

    public interface IViewFlowController : 
        IDisposable,
        ILifeTimeContext
    {
        void Activate(IViewLayoutContainer map);

    }
}