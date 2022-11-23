namespace UniGame.ViewSystem.Runtime
{
    using System;
    using global::UniGame.Core.Runtime;

    public interface IViewFlowController : 
        IDisposable,
        ILifeTimeContext
    {
        void Activate(IViewLayoutContainer map);

    }
}