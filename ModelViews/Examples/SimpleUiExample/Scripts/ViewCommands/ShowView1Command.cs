namespace UniGame.UiSystem.ModelViews.Examples.SimpleUiExample.Scripts.ViewCommands
{
    using System;
    using Examples.Scripts;
    using Runtime.Flow;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime;
    using UniRx;
    using UnityEngine;
    using ViewModels;

    [Serializable]
    public class ShowView1Command : DemoViewCommand
    {

        protected override void OnExecute(ILifeTime lifeTime)
        {
            var commandLifeTime = new LifeTimeDefinition();
            var viewModel1 = new  DemoViewModel1();

            lifeTime.AddCleanUpAction(() => commandLifeTime.Terminate());
            
            Show(viewModel1,commandLifeTime);
        }

        private void Show(DemoViewModel1 model,ILifeTime lifeTime)
        {
            
            var viewHandle = model.AsView();
            var clickStream = Observable.EveryUpdate().
                Where(x => Input.GetMouseButtonDown(0)).
                Throttle(TimeSpan.FromMilliseconds(250));
            
            viewHandle.
                Do(x => GameLog.Log($"View Model {x.Model.GetType().Name} Status: {x.Status.Value}",Color.blue)).
                Subscribe().
                AddTo(lifeTime);
            
            viewHandle.
                Where(x => x.Status.Value == ViewStatus.Shown).
                Delay(TimeSpan.FromSeconds(2)).
                CloseView().
                Subscribe().
                AddTo(lifeTime);

            clickStream.
                Zip(viewHandle, (l, handle) => handle).
                Where(x => x.Status.Value != ViewStatus.Shown).
                Do(x => GameLog.Log("OpenWindow for DemoViewModel1")).
                OpenWindow().
                Subscribe().
                AddTo(lifeTime);
                
        }
    }
}
