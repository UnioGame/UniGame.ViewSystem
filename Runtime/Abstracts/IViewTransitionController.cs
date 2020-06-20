namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IViewFlowController : 
        IDisposable,
        ILifeTimeContext
    {
        void Activate(IViewLayoutContainer map);

    }
}