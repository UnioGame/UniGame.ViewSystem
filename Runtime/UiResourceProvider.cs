using UniGame.AddressableTools.Runtime;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Settings;
    using Core.Runtime;
    using ViewSystem.Runtime;

    public class UiResourceProvider : 
        IViewResourceProvider, 
        IViewModelTypeMap
    {
        private ViewModelTypeMap _viewModelTypeMap = new();


        public void RegisterViewReferences(IEnumerable<UiViewReference> sourceViews)
        {
            _viewModelTypeMap.RegisterViewReference(sourceViews);
        }

        public UniTask<UiViewReference> GetViewReferenceAsync(
            string viewType, 
            string skinTag = "",
            string viewName = "")
        {
            var item = _viewModelTypeMap.FindView(viewType, skinTag, viewName);
            return UniTask.FromResult(item);
        }
        
        public async UniTask<TView> LoadViewAsync<TView>(
            string viewType, 
            ILifeTime lifeTime,
            string skinTag = "",  
            string viewName = "") where TView : UnityEngine.Object
        {
            var item = await GetViewReferenceAsync(viewType, skinTag, viewName);
            var view = item.View;
            return await view.LoadAssetTaskAsync<TView>(lifeTime);
        }

        #region view model map API

        public IReadOnlyList<UiViewReference> FindViews(string viewType) => _viewModelTypeMap.FindViews(viewType);
        
        public Type GetModelType(string viewType) => _viewModelTypeMap.GetModelType(viewType);
        
        public Type GetViewModelType(string viewType) => _viewModelTypeMap.GetViewModelType(viewType);

        public Type GetViewType(string modeType) => _viewModelTypeMap.GetViewType(modeType);
        
#endregion
    }
}
