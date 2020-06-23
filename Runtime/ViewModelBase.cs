namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniRx;

    public class ViewModelBase : IViewModel
    {
        private LifeTimeDefinition  lifeTimeDefinition = new LifeTimeDefinition();
        
        public BoolReactiveProperty isActive = new BoolReactiveProperty(true);
        
        public  ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        public IReadOnlyReactiveProperty<bool> IsActive => isActive;

        public void Dispose()
        {
            lifeTimeDefinition.Terminate();
        }

    }
}