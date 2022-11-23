namespace UniGame.ViewSystem.Runtime
{
    using System;
    using global::UniGame.Core.Runtime;

    public interface IViewModel : IDisposable, ILifeTimeContext
    {
        public bool IsDisposeWithModel { get; }
    }
}