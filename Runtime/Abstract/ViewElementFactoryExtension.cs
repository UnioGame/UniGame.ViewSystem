namespace UniGame.ViewSystem.Runtime
{
    using System;
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.UISystem.Runtime.Extensions;
    using UnityEngine;

    public static class ViewElementFactoryExtension
    {
        public static async UniTask<T> Open<T>( 
            this IViewElementFactory factory,
            IViewModel viewModel,
            ILifeTime lifeTime,
            Transform parent = null,
            bool stayWorldPosition = false) where T : class, IView
        {
            return await Open<T>(factory,viewModel,lifeTime,string.Empty,parent,string.Empty,stayWorldPosition);
        }
        
        public static async UniTask<T> Open<T>( 
            this IViewElementFactory factory,
            IViewModel viewModel,
            ILifeTime lifeTime,
            string skinTag = "",
            Transform parent = null,
            string viewName = "", 
            bool stayWorld = false) where T : class, IView
        {
            var view = await factory.Open(viewModel,typeof(T),lifeTime, skinTag, parent, viewName,stayWorld) as T;
            return view;
        }
        
        public static async UniTask<IView> Open( 
            this IViewElementFactory factory,
            IViewModel viewModel,
            Type viewType,
            ILifeTime lifeTime,
            string skinTag = "",
            Transform parent = null,
            string viewName = "", 
            bool stayWorld = false)
        {
            var view = await factory.Create(viewModel, viewType.Name, skinTag, parent, viewName,stayWorld);
            view.CloseWith(lifeTime);
            view.Show();
            return view;
        }
        
        public static async UniTask<T> Open<T>( 
            this IViewElementFactory factory,
            IViewModel viewModel,
            string skinTag = "",
            Transform parent = null,
            string viewName = null) where T : class, IView
        {
            var view = await factory.Create(viewModel, typeof(T), skinTag, parent, viewName) as T;
            view.Show();
            return view;
        }

        public static async UniTask<IView> Create(this IViewElementFactory factory,
            IViewModel viewModel,
            Type viewType,
            string skinTag = "",
            Transform parent = null,
            string viewName = null,
            bool stayWorld = false)
        {
            return await factory.Create(viewModel, viewType.Name, skinTag, parent, viewName,stayWorld);
        }

        public static async UniTask<T> Create<T>( 
            this IViewElementFactory factory,
            IViewModel viewModel,
            ILifeTime lifeTime,
            Transform parent = null,
            bool stayWoorldPosition = false) where T : class, IView
        {
            var view = await Create<T>(factory,viewModel,lifeTime, string.Empty, parent, string.Empty,stayWoorldPosition);
            return view;
        }
        
        public static async UniTask<T> Create<T>( 
            this IViewElementFactory factory,
            Transform parent = null,
            string skinTag = "",
            string viewName = null,
            bool stayWorldPosition = false,
            ILifeTime ownerLifeTime = null) where T : class, IView
        {
            var view = await factory.Create(typeof(T),parent,skinTag,viewName,stayWorldPosition,ownerLifeTime);
            return view as T;
        }
        
      
        public static async UniTask<T> Create<T>( 
            this IViewElementFactory factory,
            IViewModel viewModel,
            ILifeTime lifeTime,
            string skinTag = "",
            Transform parent = null,
            string viewName = "", 
            bool stayWorld = false) where T : class, IView
        {
            var view = await factory.Create(viewModel, typeof(T), skinTag, parent, viewName,stayWorld) as T;
            lifeTime.AddCleanUpAction(() => view?.Close());
            return view;
        }
        
        public static async UniTask<T> Create<T>( 
            this IViewElementFactory factory,
            IViewModel viewModel,
            string skinTag = "",
            Transform parent = null,
            string viewName = null) where T : class, IView
        {
            var view = await factory.Create(viewModel, typeof(T), skinTag, parent, viewName) as T;
            return view;
        }
    }
}