using UniModules.AddressableTools.Pooling;
using UniModules.UniCore.Runtime.ObjectPool.Runtime;
using UniModules.UniGame.AddressableTools.Runtime.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using Addressables.Reactive;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    
    using Object = UnityEngine.Object;

    
    
    public static class ViewPoolMap
    {
        
    }
    
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

            var viewLifeTime = LifeTime.Create();
            //load view source by filter parameters
            var result       = await resourceProvider.GetViewReferenceAsync(viewType,skinTag, viewName:viewName);
            //create view instance
            var view = await Create(result,viewLifeTime, parent,stayWorldPosition);
            
            //if loading failed release resource immediately
            if (view == null) {
                viewLifeTime.Despawn();
                GameLog.LogError($"Factory {this.GetType().Name} View of Type {viewType?.Name} not loaded");
                return null;
            }

            view.LifeTime.AddCleanUpAction(() => viewLifeTime.Despawn());

            return view;
        }
        
        /// <summary>
        /// create view instance
        /// </summary>
        protected virtual async UniTask<IView> Create(AssetReferenceGameObject asset,LifeTime lifeTime, Transform parent = null, bool stayPosition = false)
        {
            if (asset.RuntimeKeyIsValid() == false) return null;

            var sourceView = await asset.LoadAssetTaskAsync(lifeTime);
            var viewTransform = sourceView.transform;
            var gameObjectView = sourceView.HasCustomPoolLifeTimeFor()
                ? sourceView.SpawnActive(viewTransform.position, viewTransform.rotation, parent, stayPosition) 
                : Object.Instantiate(sourceView, parent, stayPosition);
            //create instance of view
            var view = gameObjectView.GetComponent<IView>();
            return view;
        }
    }
}
