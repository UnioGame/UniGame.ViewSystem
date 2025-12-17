using System;

namespace UniGame.UiSystem.Runtime
{
    using UniGame.Runtime.DataFlow;
    using Core.Runtime;
    using R3;
    using ViewSystem.Runtime;
    using ReactiveCommand = R3.ReactiveCommand;
    
    [Serializable]
    public class ViewModel : IViewModel
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
            _lifeTime.Terminate();
            GC.SuppressFinalize(this);
        }

        ~ViewModel()
        {
            _close?.Dispose();
            _lifeTime?.Terminate();
        }
    }
    
    [Serializable]
    public class BaseViewModel : ViewModel
    {
        
    }
}