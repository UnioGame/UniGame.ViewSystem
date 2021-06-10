namespace UniGame.UiSystem.Runtime
{
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;

    public class ViewModelBase : IViewModel
    {
        private readonly LifeTimeDefinition   _lifeTimeDefinition = new LifeTimeDefinition();
        private readonly BoolReactiveProperty _isActive           = new BoolReactiveProperty(true);

        public  ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        public virtual bool IsDisposeWithModel => true;
        
        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;

        public void Dispose()
        {
            _lifeTimeDefinition.Terminate();
        }
    }
}