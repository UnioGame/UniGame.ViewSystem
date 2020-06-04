using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using Addressables.Reactive;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UniRx.Async;
    using Object = UnityEngine.Object;

    public class ViewFactory : IViewFactory
    {
        private readonly IViewResourceProvider<Component> resourceProvider;
        
        public ViewFactory(IViewResourceProvider<Component> viewResourceProvider)
        {
            resourceProvider = viewResourceProvider;
        }

        public async UniTask<IView> Create(Type viewType, string skinTag = "", Transform parent = null) 
        {
            //load View resource
            var result = await resourceProvider.
                LoadViewAsync(viewType,skinTag).
                ToAddressableUniTask();

            var disposable = result.disposable;

            //create view instance
            var view = Create(result.value, parent);
            
            //if loading failed release resource immediately
            if (view == null) {
                GameLog.LogError($"Factory {this.GetType().Name} View of Type {viewType?.Name} not loaded");
                disposable.Dispose();
                return null;
            }
            
            //bind resource lifetime to view
            view.LifeTime.AddDispose(disposable);

            return view;
        }
        
        public async UniTask<T> Create<T>(string skinTag = "", Transform parent = null) 
            where T : Component, IView
        {
            var handler = await Create(typeof(T), skinTag, parent);
            var result = handler as T;
            
            if (result == null) {
                GameLog.LogError($"View type mismatch Request type {typeof(T).Name} : ResultType {handler?.GetType().Name}");
                handler?.Destroy();
            }
            
            return result;
        }
        
        /// <summary>
        /// create view instance
        /// </summary>
        protected virtual IView Create(Component asset, Transform parent = null)
        {
            if (asset == null) return null;
            //create instance of view
            var view = Object.
                Instantiate(asset.gameObject, parent).
                GetComponent<IView>();
            return view;
        }
    }
}
