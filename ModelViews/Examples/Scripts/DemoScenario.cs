using UnityEngine;

namespace UniGame.UiSystem.ModelViews.Examples.SimpleUiExample.Scripts
{
    using System;
    using System.Collections.Generic;
    using Examples.Scripts;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;

    public class DemoScenario : MonoBehaviour,IDisposable
    {
        [SerializeReference]
        public List<DemoViewCommand> commands = new List<DemoViewCommand>();

        public LifeTimeDefinition _lifeTime = new LifeTimeDefinition();
        
        public void Execute()
        {
            foreach (var command in commands) {
                command.
                    Execute().
                    AddTo(_lifeTime);
            }
        }

        public void Dispose()
        {
            _lifeTime.Terminate();
        }
    }
}
