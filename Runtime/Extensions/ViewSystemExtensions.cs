using Cysharp.Threading.Tasks;
using UniGame.UiSystem.Runtime.Settings;
using UniModules.UniCore.Runtime.DataFlow;
using UniGame.AddressableTools.Runtime;
using UniGame.Core.Runtime;
using UniGame.Context.Runtime;
using UniModules.UniGame.ViewSystem.Runtime.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniModules.UniGame.ViewSystem.Runtime.Extensions
{
    public static class ViewSystemExtensions 
    {
        public static async UniTask<ILifeTime> Warmup(this AssetReferenceViewSettings settingsReference,ILifeTime lifeTime)
        {
            return await WarmupInternal(settingsReference, lifeTime);
        }
        
        public static async UniTask<ILifeTime> Warmup(this AssetReferenceViewSettings settingsReference,Scene scene)
        {
            return await WarmupInternal(settingsReference, scene);
        }
        
        public static async UniTask<ILifeTime> Warmup(this AssetReferenceViewSettings settingsReference,GameObject gameObject)
        {
            return await WarmupInternal(settingsReference, gameObject);
        }
        
        public static async UniTask<ILifeTime> Warmup(this IViewsSettings settings,Scene scene)
        {
            return await settings.Warmup(scene.GetLifeTime());
        }
        
        public static async UniTask<ILifeTime> Warmup(this IViewsSettings settings,GameObject gameObject)
        {
            return await settings.Warmup(gameObject.GetLifeTime());
        }
        
        public static async UniTask<ILifeTime> Warmup(this IViewsSettings settings,ILifeTime lifeTime,int preloadCount = 0)
        {
            var viewHandles = settings.Views;
            
            var views = await UniTask
                .WhenAll(viewHandles.Select(x => WarmUpUiReference(x,lifeTime,preloadCount)))
                .AttachExternalCancellation(lifeTime.TokenSource);
            
            return lifeTime;
        }

        public static async UniTask<GameObject> WarmUpUiReference(UiViewReference reference, ILifeTime lifeTime,int preloadCount = 0)
        {
            var count = preloadCount >= reference.PoolingPreloadCount ? preloadCount : reference.PoolingPreloadCount;
            var view  = reference.View;
            var asset = await view.CreatePool(lifeTime,count);
            return asset;
        }

        private static async UniTask<ILifeTime> WarmupInternal(this AssetReferenceViewSettings settingsReference,object lifeTimeObject)
        {
            var lifeTime = lifeTimeObject.GetLifeTime();
            var settings = await settingsReference.LoadAssetTaskAsync<ScriptableObject, IViewsSettings>(lifeTime);
            var viewSettings = settings.result;
            if (viewSettings == null)
            {
                return LifeTime.TerminatedLifetime;
            }
            
            return await viewSettings.Warmup(lifeTime);
        }
    }
}
