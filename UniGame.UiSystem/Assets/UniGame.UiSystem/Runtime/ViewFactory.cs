using UnityEngine;

namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using Abstracts;
    using Taktika.Addressables.Reactive;
    using UniCore.Runtime.ProfilerTools;
    using UniRx.Async;

    public class ViewFactory : IViewFactory
    {
        private readonly IViewResourceProvider resourceProvider;
        
        public ViewFactory(IViewResourceProvider viewResourceProvider)
        {
            resourceProvider = viewResourceProvider;
        }

        public async UniTask<T> Create<T>(string skinTag = "") 
            where T : Component, IView
        {
            //load View resource
            var result = await resourceProvider.
                LoadViewAsync<T>(skinTag).
                ToAddressableUniTask();

            var asset      = result.value;
            var disposable = result.disposable;
            
            //if loading failed release resource immediately
            if (asset == null) {
                GameLog.LogError($"{nameof(ViewController)} View of Type {typeof(T).Name} not loaded");
                disposable.Dispose();
                return null;
            }

            //create view instance
            var view = Create(asset);
            //bind resource lifetime to view
            view.LifeTime.AddDispose(disposable);

            return view;

        }
        
        protected virtual TView Create<TView>(TView asset) where TView : Component, IView
        {
            //create instance of view
            var view = Object.
                Instantiate(asset.gameObject).
                GetComponent<TView>();

            return view;
        }
    }
}
