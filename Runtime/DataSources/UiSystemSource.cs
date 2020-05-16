namespace UniModules.UniGame.UISystem.Runtime.DataSources
{
    using System;
    using Context.Runtime.Abstract;
    using global::UniGame.ModelViewsMap.Runtime.Settings;
    using global::UniGame.UiSystem.Runtime;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniRx.Async;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    [CreateAssetMenu(menuName = "UniGame/UiSystem/UiSystemSource", fileName = "UiSystemSource")]
    public class UiSystemSource : AsyncContextDataSource
    {
        [SerializeField] 
        private AssetReferenceUiSystem uiSystemSource;

        private static IGameViewSystem uiSystemAsset;
        
        public async UniTask<IGameViewSystem> LoadSystem()
        {
            if (uiSystemAsset != null) {
                return uiSystemAsset;
            }
            
            var uiSystem = await uiSystemSource.LoadGameObjectAssetTaskAsync(LifeTime);

            var uiAsset = Instantiate(uiSystem);
            DontDestroyOnLoad(uiAsset.gameObject);

            LifeTime.AddCleanUpAction(() => {
                if (uiAsset) {
                    Object.Destroy(uiAsset.gameObject);
                }
            });
                        
            uiSystemAsset = uiAsset.ViewSystem;
            return uiSystemAsset;
        }
        
        public sealed override async UniTask<IContext> RegisterAsync(IContext context)
        {
            var uiSystem = await LoadSystem();
            context.Publish(uiSystem);
            return context;
        }
    }
}
