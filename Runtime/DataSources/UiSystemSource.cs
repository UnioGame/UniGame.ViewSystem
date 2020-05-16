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

        private static GameViewSystemAsset uiSystemAsset;
        
        public async UniTask<IGameViewSystem> LoadSystem()
        {
            if (uiSystemAsset) {
                return uiSystemAsset;
            }
            
            var uiSystem = await uiSystemSource.LoadGameObjectAssetTaskAsync(LifeTime);
            
            uiSystemAsset = Instantiate(uiSystem);
            
            DontDestroyOnLoad(uiSystemAsset.gameObject);

            LifeTime.AddCleanUpAction(() => {
                if (uiSystemAsset) {
                    Object.Destroy(uiSystemAsset.gameObject);
                }
            });
            
            return uiSystem;
        }
        
        public sealed override async UniTask<IContext> RegisterAsync(IContext context)
        {
            var uiSystem = await LoadSystem();
            context.Publish(uiSystem);
            return context;
        }
    }
}
