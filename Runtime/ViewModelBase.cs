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
        private readonly LifeTimeDefinition   _lifeTimeDefinition = new LifeTimeDefinition();
        private readonly BoolReactiveProperty _isActive           = new BoolReactiveProperty(true);
        private          bool                 _disposeWithModel   = true;
        private ReactiveCommand _close => new ReactiveCommand();

        public  ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        public virtual bool IsDisposeWithModel
        {
            get => _disposeWithModel;
            protected set => _disposeWithModel = value;
        }

        public IReactiveCommand<Unit> Close => _close;
        
        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;

        public IViewModel DisposeByModel(bool disposeWithModel)
        {
            IsDisposeWithModel = disposeWithModel;
            return this;
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void Dispose()
        {
            if (_lifeTimeDefinition.IsTerminated) return;
            
            _close.Dispose();
            _lifeTimeDefinition.Terminate();
            GC.SuppressFinalize(this);
        }

        ~ViewModelBase()
        {
            _close.Dispose();
            _lifeTimeDefinition?.Terminate();
        }
    }
}