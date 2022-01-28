using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.Extensions;

namespace UniGame.UiSystem.Runtime
{
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using System;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.Common;
    using UniModules.UniGame.Core.Runtime.DataFlow.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using Object = UnityEngine.Object;
    
    public class ViewFactory : IViewFactory
    {
        private readonly AsyncLazy _readyStatus;
        private readonly IViewResourceProvider resourceProvider;
        
        public ViewFactory(
            AsyncLazy readyStatus,
            IViewResourceProvider viewResourceProvider)
        {
            _readyStatus = readyStatus;
            resourceProvider = viewResourceProvider;
        }

        public async UniTask<IView> Create(
            Type viewType, 
            string skinTag = "", 
            Transform parent = null, 
            string viewName = "",
            bool stayWorldPosition = false)
        {
            await _readyStatus;

            var viewDisposable = new DisposableLifetime();
            viewDisposable.Initialize();
            
            //load view source by filter parameters
            var result       = await resourceProvider.GetViewReferenceAsync(viewType,skinTag, viewName:viewName);
            //create view instance
            var viewResult   = await Create(result,viewDisposable.LifeTime, parent,stayWorldPosition);
            var view         = viewResult.View;
            var viewLifeTime = viewResult.AssetLifeTime;
            
            //if loading failed release resource immediately
            if (view == null) {
                viewDisposable.Dispose();
                GameLog.LogError($"Factory {this.GetType().Name} View of Type {viewType?.Name} not loaded or cancelled");
                return null;
            }

            viewLifeTime.AddDispose(viewDisposable);
            return view;
        }
        
        /// <summary>
        /// create view instance
        /// </summary>
        protected virtual async UniTask<ViewResult> Create(AssetReferenceGameObject asset,ILifeTime lifeTime, Transform parent = null, bool stayPosition = false)
        {
            if (asset.RuntimeKeyIsValid() == false) return new ViewResult();

            var sourceView = await asset.LoadAssetTaskAsync(lifeTime);
            
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
        
        public struct ViewResult
        {
            public IView     View;
            public ILifeTime AssetLifeTime;
        }
    }
}
