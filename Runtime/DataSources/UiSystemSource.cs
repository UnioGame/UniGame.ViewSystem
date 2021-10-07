namespace UniModules.UniGame.UISystem.Runtime.DataSources
{
    using System;
    using System.Threading;
    using Abstract;
    using Context.Runtime.Abstract;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using global::UniGame.ModelViewsMap.Runtime.Settings;
    using global::UniGame.UiSystem.Runtime;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.GameFlow.Runtime.Services;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/UiSystemSource", fileName = "UiSystemSource")]
    public class UiSystemSource : DataSourceAsset<IGameViewSystem>
    {
        [SerializeField] 
        private AssetReferenceUiSystem uiSystemSource;


        protected override async UniTask<IGameViewSystem> CreateInternalAsync(IContext context)
        {
            var uiSystem = await uiSystemSource.LoadGameObjectAssetTaskAsync(LifeTime);
            var uiAsset  = Instantiate(uiSystem.gameObject).GetComponent<GameViewSystemAsset>();
            
            DontDestroyOnLoad(uiAsset.gameObject);

            LifeTime.AddCleanUpAction(() =>
            {
                if (!uiAsset)
                    return;
                Destroy(uiAsset.gameObject);
            });

            return uiAsset;
        }
    }
}
