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
    using UniModules.UniGame.Core.Runtime.Common;
    using UniModules.UniGame.Core.Runtime.DataFlow.Extensions;
    using Core.Runtime;
    using ViewSystem.Runtime;
    using Object = UnityEngine.Object;
    
    public class ViewFactory : IViewFactory
    {
        private readonly AsyncLazy                      _readyStatus;
        private readonly IViewResourceProvider          _resourceProvider;
        private readonly Dictionary<string, GameObject> _assetReferenceMap;

        public ViewFactory(
            AsyncLazy readyStatus,
            IViewResourceProvider viewResourceProvider)
        {
            _assetReferenceMap = new Dictionary<string, GameObject>();
            
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

            var viewDisposable = new DisposableLifetime();
            viewDisposable.Initialize();

            //load view source by filter parameters
            var item = await _resourceProvider.GetViewReferenceAsync(viewId, skinTag, viewName);

            if (item == null)
            {
                Debug.LogError($"{nameof(UiResourceProvider)} ITEM MISSING {viewId} skin:{skinTag} viewName:{viewName}");
                return null;
            }

            var result = item.View;
 
            //create view instance
            var viewResult   = await Create(result,viewDisposable.LifeTime, parent,stayWorldPosition);
            var view         = viewResult.View;
            var viewLifeTime = viewResult.AssetLifeTime;
            
            //if loading failed release resource immediately
            if (view == null) {
                viewDisposable.Dispose();
                GameLog.LogError($"Factory {this.GetType().Name} View of Type {viewId} not loaded or cancelled");
                return null;
            }

            viewLifeTime.AddDispose(viewDisposable);
            view.SetSourceName(viewId,item.ViewName);
            
            return view;
        }
        
        /// <summary>
        /// create view instance
        /// </summary>
        protected virtual async UniTask<ViewResult> Create(
            AssetReferenceGameObject asset,
            ILifeTime lifeTime,
            Transform parent = null,
            bool stayPosition = false)
        {
            if (asset.RuntimeKeyIsValid() == false) return new ViewResult();

            var sourceView = await LoadAssetReferenceAsset(asset, lifeTime); // await asset.LoadAssetTaskAsync(lifeTime);

            //operation was cancelled
            if(sourceView == null) return new ViewResult();

            var viewTransform = sourceView.transform;
            
            var gameObjectView = sourceView.HasCustomPoolLifeTimeFor()
                ? sourceView.SpawnActive(viewTransform.position, viewTransform.rotation, parent, stayPosition) 
                : Object.Instantiate(sourceView, parent, stayPosition);

            //create instance of view
            var view          = gameObjectView.GetComponent<IView>();
            var assetLifeTime = gameObjectView.GetAssetLifeTime();
            
            return new ViewResult()
            {
                View = view,
                AssetLifeTime = assetLifeTime
            };
        }

        protected async UniTask<GameObject> LoadAssetReferenceAsset(AssetReferenceGameObject asset,ILifeTime lifeTime)
        {
            if (_assetReferenceMap.TryGetValue(asset.AssetGUID, out var gameObject) && gameObject != null)
                return gameObject;
            
            var sourceView = await asset.LoadAssetTaskAsync(lifeTime);
            _assetReferenceMap[asset.AssetGUID] = sourceView;
            return sourceView;
        }

        public struct ViewResult
        {
            public IView     View;
            public ILifeTime AssetLifeTime;
        }
    }
}
