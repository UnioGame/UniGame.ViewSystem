namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    public interface IViewFlowController : 
        IDisposable,
        ILifeTimeContext
    {
        void Activate(IViewLayoutContainer map);

    }
}