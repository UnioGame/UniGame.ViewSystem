namespace UniGame.UiSystem.Runtime
{
    using System;
    using Core.Runtime;
    using UniGame.Runtime.DataFlow;

    [Serializable]
    public class ViewData : ILifeTimeContext
    {
        private readonly LifeTimeDefinition   _lifeTimeDefinition = new LifeTimeDefinition();

        public  ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        public void Dispose()
        {
            if (_lifeTimeDefinition.IsTerminated)
                return;
            _lifeTimeDefinition.Terminate();
            GC.SuppressFinalize(this);
        }

        ~ViewData()  => _lifeTimeDefinition.Terminate();
    }
}