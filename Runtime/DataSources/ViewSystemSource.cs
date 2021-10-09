namespace UniModules.UniGame.UISystem.Runtime.DataSources
{
    using System;
    using System.Threading;
    using Abstract;
    using Context.Runtime.Abstract;
    using Core.Runtime.Extension;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using global::UniGame.ModelViewsMap.Runtime.Settings;
    using global::UniGame.UiSystem.Runtime;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.GameFlow.Runtime.Services;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/" + nameof(ViewSystemSource), fileName = nameof(ViewSystemSource))]
    public class ViewSystemSource : DataSourceAsset<IGameViewSystem>
    {
        [FormerlySerializedAs("uiSystemSource")]
        [SerializeField] 
        private AssetReferenceUiSystem viewSystemSource;


        protected override async UniTask<IGameViewSystem> CreateInternalAsync(IContext context)
        {
            var viewSystemAsset = await viewSystemSource.LoadGameObjectAssetTaskAsync(LifeTime);
            var viewObject = Instantiate(viewSystemAsset.gameObject);
            var viewAsset  = viewObject.GetComponent<GameViewSystemAsset>();
            
            DontDestroyOnLoad(viewObject);
            viewObject.DestroyWith(LifeTime);

            await UniTask.WaitUntil(() => viewAsset.IsReady == true);
            
            return viewAsset;
        }
    }
}
