using System;
using Cysharp.Threading.Tasks;
using UniGame.UiSystem.Runtime.Settings;
using UniGame.Core.Runtime;
using UniModules.UniGame.UiSystem.Runtime;
using UniGame.ViewSystem.Runtime;
using UniGame.ViewSystem.Runtime.Abstract;
using UniModules.UniGame.ViewSystem.Runtime.Extensions;
using UnityEngine;

namespace UniGame.ViewSystem.Runtime.Extensions
{
    using Abstract;

    public static class ViewsFactoryExtensions
    {
        
        public static IGameViewSystem Current { get; private set; }
        

        public static async UniTask<IView> OpenWindow<TView>(this IContext context,string skinTag = "", string viewName = null)
        {
            return await CreateView<TView>(context,ViewType.Window, skinTag, null, viewName);
        }

        public static async UniTask<IView> OpenScreen<TView>(this IContext context,string skinTag = "", string viewName = null)
        {
            return await CreateView<TView>(context,ViewType.Screen, skinTag, null, viewName);
        }

        public static async UniTask<IView> OpenOverlay<TView>(this IContext context,string skinTag = "", string viewName = null)
        {
            return await CreateView<TView>(context,ViewType.Overlay, skinTag, null, viewName);
        }
        
        public static async UniTask<IView> Create<TView>(
            this IContext context,
            string skinTag = "",
            Transform parent = null,
            string viewName = null,
            bool stayWorldPosition = false)
        {
            return await CreateView(Current,context,typeof(TView), ViewType.None, skinTag, viewName,parent,stayWorldPosition);
        }

        public static IViewLayoutProvider MakeActive(this IGameViewSystem viewSystem)
        {
            Current = viewSystem;

            var lifeTime = Current.LifeTime;
            
            lifeTime.AddCleanUpAction(() =>
            {
                if (Current != viewSystem)
                    return;
                Current = null;
            });

            return viewSystem;
        }
        
        public static bool TryMakeActive(this IGameViewSystem viewSystem)
        {
            if (Current != null)
                return false;
            
            MakeActive(viewSystem);
            
            return true;
        }
        
        #region private methods
        
        private static async UniTask<IView> CreateView<TView>(
            IContext context,
            ViewType viewLayoutType,
            string skinTag = "",
            Transform parent = null,
            string viewName = null,
            bool stayWorldPosition = false)
        {
            return await CreateView(Current,context,typeof(TView), viewLayoutType, skinTag, viewName,parent,stayWorldPosition);
        }
        
        private static async UniTask<IView> CreateView(
            IGameViewSystem viewSystem,
            IContext context,
            Type viewType,
            ViewType viewLayoutType,
            string skinTag = "",
            string viewName = null,
            Transform parent = null,
            bool stayWorldPosition = false)
        {
#if UNITY_EDITOR
            if (Current == null)
            {
                throw new NullReferenceException("ViewSystem must be initialized before usage");
            }
#endif
            var typeMap = viewSystem.ModelTypeMap;
            var views = typeMap.FindViewsByType(viewType);
            var modelReference = views.SelectReference(skinTag, viewName);
            var modelType = modelReference.ViewModelType;
            
            var model = await viewSystem.Create(context,modelType);

            return viewLayoutType switch
            {
                ViewType.None => await viewSystem.Create(model, viewType, skinTag, parent, viewName, stayWorldPosition),
                ViewType.Screen => await viewSystem.OpenScreen(model, viewType, skinTag, viewName),
                ViewType.Window => await viewSystem.OpenWindow(model, viewType, skinTag, viewName),
                ViewType.Overlay => await viewSystem.OpenOverlay(model, viewType, skinTag, viewName),
                _ => await viewSystem.OpenScreen(model, viewType, skinTag, viewName)
            };
        }

        private static UiViewReference SelectReference(IViewModelTypeMap viewModelTypeMap,Type viewType,string tag,string name)
        {
            return new UiViewReference();
        }
        
        #endregion
        
    }
}
