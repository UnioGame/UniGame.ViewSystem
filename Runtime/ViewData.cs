namespace UniGame.UiSystem.Runtime
{
    using System;
    using Core.Runtime;
    using UniGame.Runtime.DataFlow;

    [Serializable]
    public class ViewData : ILifeTimeContext
    {
        private readonly LifeTime   _lifeTime = new();

        public  ILifeTime LifeTime => _lifeTime;

        public void Dispose()
        {
            if (_lifeTime.IsTerminated)
                return;
            _lifeTime.Terminate();
            GC.SuppressFinalize(this);
        }

        ~ViewData()  => _lifeTime.Terminate();
    }
}