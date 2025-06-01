using System;

namespace UniGame.UiSystem.Runtime
{
    using UniModules.UniCore.Runtime.DataFlow;
    using Core.Runtime;
    using ViewSystem.Runtime;
    using UniRx;

    [Serializable]
    public class ViewModelBase : IViewModel
    {
        private LifeTime _lifeTime = new();
        private ReactiveCommand _close = new();

        public ILifeTime LifeTime => _lifeTime;
        
        public IReactiveCommand<Unit> CloseCommand => _close;

        public void Close() => _close.Execute();
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void Dispose()
        {
            if (_lifeTime.IsTerminated) return;

            _close.Dispose();
            _lifeTime.Release();
            GC.SuppressFinalize(this);
        }

        ~ViewModelBase()
        {
            _close.Dispose();
            _lifeTime?.Release();
        }
    }
}