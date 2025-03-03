namespace Game.Modules.ViewSystem.ZenjectViewModule
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.UiSystem.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UnityEngine;
    using Zenject;

    [Serializable]
    public class ZenjectViewFactory  : IViewFactory
    {
        public ViewFactory _viewFactory;
        public DiContainer _container;
        
        public ZenjectViewFactory(DiContainer container,AsyncLazy readyStatus, IViewResourceProvider viewResourceProvider)
        {
            _container = container;
            _viewFactory = new ViewFactory(readyStatus, viewResourceProvider);
        }
        
        public async UniTask<IView> Create(string viewId, 
            string skinTag = "", 
            Transform parent = null, 
            string viewName = null,
            bool stayWorldPosition = false)
        {
            var view = await _viewFactory.Create(viewId, skinTag, parent, viewName, stayWorldPosition);
            if (view == null || view.GameObject == null) return view;
            var viewObject = view.GameObject;
            
            _container.InjectGameObject(viewObject);
            return view;
        }
    }
}