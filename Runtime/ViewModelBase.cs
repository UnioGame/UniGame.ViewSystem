using System;

namespace UniGame.UiSystem.Runtime
{
    using UniGame.Runtime.DataFlow;
    using Core.Runtime;
    using R3;
    using ViewSystem.Runtime;
     

    [Serializable]
    public class ViewModelBase : IViewModel
    {
        private LifeTime _lifeTime = new();
        private ReactiveCommand _close = new();

        public ILifeTime LifeTime => _lifeTime;
        
        public ReactiveCommand CloseCommand => _close;

        public void Close() => _close.Execute(Unit.Default);
        
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