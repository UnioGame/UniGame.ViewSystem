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

        public async UniTask<IView> Create(Type viewType, string skinTag = "", Transform parent = null, string viewName = null) 
        {
            //load View resource
            var result = await resourceProvider.
                LoadViewAsync(viewType,skinTag, viewName:viewName).
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
