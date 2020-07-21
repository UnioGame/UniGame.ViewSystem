using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using Addressables.Reactive;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UniRx;
    
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
            var viewObservable = resourceProvider.
                LoadViewAsync(viewType,skinTag, viewName:viewName);
            
            //load View resource
            var result = await viewObservable.First();
            //create view instance
            var view = Create(result, parent);
            
            //if loading failed release resource immediately
            if (view == null) {
                viewObservable.Dispose();
                GameLog.LogError($"Factory {this.GetType().Name} View of Type {viewType?.Name} not loaded");
                return null;
            }

            viewObservable.AddTo(view.LifeTime);
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
