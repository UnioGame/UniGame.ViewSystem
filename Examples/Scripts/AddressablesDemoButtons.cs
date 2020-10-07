using UnityEngine;

namespace UniModules.UniGame.UiSystem.Examples.BaseUiManager
{
    using System.Collections.Generic;
    using AddressableTools.Runtime.Extensions;
    using global::UniCore.Runtime.ProfilerTools;
    using global::UniGame.Addressables.Reactive;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine.AddressableAssets;

    public class AddressablesDemoButtons : MonoBehaviour
    {
        
        public AssetReference demoAsset;

        public AssetReference demoAsset2;

        private LifeTimeDefinition lifeTimeDefinition = new LifeTimeDefinition();
        public List<Object> firstAssets = new List<Object>();
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void LoadAddressables()
        {
            demoAsset.ToObservable<GameObject>().
                Do(x => firstAssets.Add(x)).
                Do(x => {
                    for (int i = 0; i < firstAssets.Count - 1; i++) {
                        var a1 = firstAssets[i];
                        var a2 = firstAssets[i + 1];

                        GameLog.Log($"AS1 == AS2 {a1 == a2}");
                    }
                }).
                Subscribe(x => GameLog.Log($"LOAD 1 ADRS {x.name}")).
                AddTo(lifeTimeDefinition.LifeTime);
            
            LoadAddressablesAsync();
        }
        
        public async void LoadAddressablesAsync()
        {
            var result  = await demoAsset2.LoadAssetTaskAsync<Object>(lifeTimeDefinition.LifeTime);
            var result2 = await demoAsset2.LoadAssetTaskAsync<GameObject>(lifeTimeDefinition.LifeTime);
            GameLog.Log($"LOAD 2 OBJECT ADRS {result.name}");
            GameLog.Log($"LOAD 2 GAMEOBJECT ADRS {result2.name}");
        }
        
        
        private void Start()
        {
            LoadAddressables();    
        }

        private void OnDisable()
        {
            lifeTimeDefinition.Terminate();
        }
    }
}
