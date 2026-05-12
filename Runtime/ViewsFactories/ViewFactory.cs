using UniGame.AddressableTools.Runtime;

namespace UniGame.UiSystem.Runtime
{
    using UniGame.Runtime.ObjectPool;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.Runtime.ObjectPool.Extensions;
    using Common;
    using Core.Runtime;
    using UniGame.Runtime.DataFlow;
    using ViewSystem.Runtime;
    using Object = UnityEngine.Object;
    
    public class ViewFactory : IViewFactory
    {
        private AsyncLazy                      _readyStatus;
        private IViewResourceProvider          _resourceProvider;
        private LifeTime _lifeTime = new();

        public ViewFactory(AsyncLazy readyStatus, IViewResourceProvider viewResourceProvider)
        {
            _readyStatus       = readyStatus;
            _resourceProvider = viewResourceProvider;
            _resourceProvider = viewResourceProvider;
        }

        public async UniTask<IView> Create(
            string viewId,
            string skinTag = "",
            Transform parent = null,
            string viewName = "",
            bool stayWorldPosition = false)
        {
            await _readyStatus;

            //load view source by filter parameters
            var viewReference = await _resourceProvider.GetViewReferenceAsync(viewId, skinTag, viewName);

            if (viewReference == null)
            {
                Debug.LogError($"{nameof(UiResourceProvider)} ITEM MISSING {viewId} skin:{skinTag} viewName:{viewName}");
                return null;
            }

            var result = viewReference.View;
            var loadLifeTime = new LifeTime();
            var usePooling = viewReference.UsePooling;
            var keepInMemory = viewReference.KeepInMemory;
            
            //create view instance
            var viewResult   = await Create(result,loadLifeTime, parent,stayWorldPosition,usePooling);
            
            var view         = viewResult.View;
            var viewLifeTime = viewResult.AssetLifeTime;
            
            //if loading failed release resource immediately
            if (view == null) {
                loadLifeTime.Dispose();
                GameLog.LogError($"Factory {GetType().Name} View of Type {viewId} not loaded or cancelled");
                return null;
            }

            var memoryLifeTime = viewLifeTime;
            
            //if view need to be keeped in memory even if pooling is disabled
            if(!usePooling && keepInMemory)
                memoryLifeTime = _lifeTime;
            
            memoryLifeTime.AddDispose(loadLifeTime);
            view.SetSourceName(viewId,viewReference.ViewName);
            
            return view;
        }
        
        /// <summary>
        /// create view instance
        /// </summary>
        protected virtual async UniTask<ViewResult> Create(
            AssetReferenceGameObject asset,
            ILifeTime lifeTime,
            Transform parent = null,
            bool stayPosition = false,
            bool usePooling = false,
            int preloadCount = 0)
        {
            if (!asset.RuntimeKeyIsValid()) return new ViewResult();

            var sourceView = await LoadAssetReferenceAsset(asset, lifeTime);

            //operation was cancelled
            if(sourceView == null) return new ViewResult();

            var viewTransform = sourceView.transform;
            var hasPool = sourceView.HasPool();

            if (usePooling && !hasPool)
                hasPool = sourceView.CreatePool(preloadCount);
            
            var takeFromPool = usePooling || hasPool;
            
            var gameObjectView = takeFromPool
                ? sourceView.Spawn(viewTransform.position, viewTransform.rotation, parent, stayPosition) 
                : Object.Instantiate(sourceView, parent, stayPosition);

            var isActive = gameObjectView.activeSelf;
            
            if (isActive)
            {
                gameObjectView.SetActive(false);
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate,lifeTime.Token);
                gameObjectView.SetActive(true);
            }
            
            //create instance of view
            var view          = gameObjectView.GetComponent<IView>();
            var assetLifeTime = gameObjectView.GetAssetLifeTime();
            
            return new ViewResult()
            {
                Source = sourceView,
                View = view,
                AssetLifeTime = assetLifeTime
            };
        }

        protected async UniTask<GameObject> LoadAssetReferenceAsset(AssetReferenceGameObject asset,ILifeTime lifeTime)
        {
            var guid = asset.AssetGUID;

            var loadResult = await guid.LoadAssetReferenceAsync<GameObject>(
                lifeTime,
                false,
                lifeTime.Token);
            
            return loadResult.Result;
        }


        public void Dispose() => _lifeTime.Terminate();
        
        
        public struct ViewResult
        {
            public GameObject Source;
            public IView     View;
            public ILifeTime AssetLifeTime;
        }
    }
}
