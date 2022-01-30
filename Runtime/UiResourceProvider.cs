using UniModules.UniGame.SerializableContext.Runtime.Addressables;
using UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Abstract;
using UniModules.UniGame.ViewSystem.Runtime.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Settings;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;

    public class UiResourceProvider : 
        IViewResourceProvider, 
        IViewModelTypeMap
    {
        private ViewModelTypeMap _viewModelTypeMap = new ViewModelTypeMap();


        public void RegisterViewReferences(IEnumerable<UiViewReference> sourceViews)
        {
            _viewModelTypeMap.RegisterViewReference(sourceViews);
        }

        public async UniTask<AssetReferenceGameObject> GetViewReferenceAsync(
            Type viewType, 
            string skinTag = "", 
            bool strongMatching = true, 
            string viewName = "")
        {
            var item = _viewModelTypeMap.FindView(viewType, skinTag, viewName, strongMatching);

            if (item != null) return item.View;
            
            Debug.LogError($"{nameof(UiResourceProvider)} ITEM MISSING skin:{skinTag} type {viewType.Name}");
            return null;
        }
        
        public async UniTask<TView> LoadViewAsync<TView>(
            Type viewType, 
            ILifeTime lifeTime,
            string skinTag = "", 
            bool strongMatching = true, 
            string viewName = "") where TView : UnityEngine.Object
        {
            var item = await GetViewReferenceAsync(viewType, skinTag, strongMatching, viewName);
            return await item.LoadAssetTaskAsync<TView>(lifeTime);
        }

        public List<UniTask<TView>> LoadViewsAsync<TView>(
            Type viewType,ILifeTime lifeTime, string skinTag = null, bool strongMatching = true)
            where TView : UnityEngine.Object
        {
            var items = _viewModelTypeMap.FindViewsByType(viewType, strongMatching);

            if (items.Count <= 0)
            {
                Debug.LogError($"{nameof(UiResourceProvider)} ITEM MISSING skin:{skinTag} type {viewType.Name}");
                return null;
            }

            return items
                .Select(item => item.View.LoadAssetTaskAsync<TView>(lifeTime))
                .ToList();
        }

#region view model map API

        public IReadOnlyList<UiViewReference> FindViewsByType(Type viewType, bool strongMatching = true) => _viewModelTypeMap.FindViewsByType(viewType, strongMatching);

        public IReadOnlyList<UiViewReference> FindModelByType(Type modelType, bool strongMatching = true) => _viewModelTypeMap.FindModelByType(modelType, strongMatching);

        public Type GetModelTypeByView(Type viewType, bool strongTypeMatching = true) => _viewModelTypeMap.GetModelTypeByView(viewType, strongTypeMatching);

        public Type GetViewTypeByModel(Type modeType, bool strongTypeMatching = true) => _viewModelTypeMap.GetViewTypeByModel(modeType, strongTypeMatching);
        
#endregion
    }
}
