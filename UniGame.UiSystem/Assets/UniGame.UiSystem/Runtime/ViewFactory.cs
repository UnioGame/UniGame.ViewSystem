﻿using UnityEngine;

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

        public async UniTask<T> Create<T>(string skinTag = "", Transform parent = null) 
            where T : Component, IView
        {
            //load View resource
            var result = await resourceProvider.
                LoadViewAsync<T>(skinTag).
                ToAddressableUniTask();

            var asset      = result.value;
            var disposable = result.disposable;
            
            //if loading failed release resource immediately
            // почему логи от nameof(ViewController) хотя по факты это ViewFactory
            if (asset == null) {
                GameLog.LogError($"{nameof(ViewStackController)} View of Type {typeof(T).Name} not loaded");
                disposable.Dispose();
                return null;
            }

            //create view instance
            var view = Create(asset, parent);
            //bind resource lifetime to view
            view.LifeTime.AddDispose(disposable);

            return view;

        }
        
        // virtual который никто не переопределяет
        protected virtual TView Create<TView>(TView asset, Transform parent = null) where TView : Component, IView
        {
            //create instance of view
            var view = Object.
                Instantiate(asset.gameObject, parent).
                GetComponent<TView>();

            return view;
        }
    }
}
