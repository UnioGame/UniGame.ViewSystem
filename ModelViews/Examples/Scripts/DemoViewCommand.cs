namespace UniGame.UiSystem.ModelViews.Examples.Scripts
{
    using System;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Extension;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UnityEngine;

    [Serializable]
    public abstract class DemoViewCommand : MonoBehaviour,IViewDemoCommand
    {
        public IDisposable Execute()
        {
            var lifeTimeDefinition = new LifeTimeDefinition();
            OnExecute(lifeTimeDefinition);
            return lifeTimeDefinition.AsDisposable(x => x.Terminate());
        }

        protected abstract void OnExecute(ILifeTime lifeTime);
    }
}
