namespace UniModules.UniGame.UISystem.Runtime.DataSources
{
    using System;
    using Abstract;
    using Context.Runtime.Abstract;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using global::UniGame.ModelViewsMap.Runtime.Settings;
    using global::UniGame.UiSystem.Runtime;
    using UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/UiSystemSource", fileName = "UiSystemSource")]
    public class UiSystemSource : AsyncContextDataSource
    {
        [SerializeField] 
        private AssetReferenceUiSystem uiSystemSource;

        private static IGameViewSystem viewSystemAsset;
        private static GameObject viewSystemObject;
        
        public async UniTask<IGameViewSystem> LoadSystem()
        {
            if (viewSystemAsset != null && viewSystemObject!=null) {
                return viewSystemAsset;
            }
            
            var uiSystem = await uiSystemSource.LoadGameObjectAssetTaskAsync(LifeTime);
            var uiAsset = Instantiate(uiSystem);
            
            DontDestroyOnLoad(uiAsset.gameObject);

            LifeTime.AddCleanUpAction(() =>
            {
                if (!uiAsset)
                    return;
                
                viewSystemAsset.Cancel();
                Destroy(uiAsset.gameObject);
                viewSystemAsset = null;
            });
                        
            viewSystemAsset = uiAsset.ViewSystem;
            return viewSystemAsset;
        }
        
        public sealed override async UniTask<IContext> RegisterAsync(IContext context)
        {
            var uiSystem = await LoadSystem().AttachExternalCancellation(LifeTime.TokenSource);
            context.Publish(uiSystem);
            return context;
        }
    }
}
