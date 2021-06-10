namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;
    using Core.Runtime.Interfaces;

    public interface IViewModel : IDisposable, ILifeTimeContext
    {
        public bool IsDisposeWithModel { get; }
    }
}