using System;
using System.Collections.Generic;
using System.Linq;
using UniGame.UiSystem.Runtime.Settings;
using UniModules.UniGame.ViewSystem.Runtime.Extensions;

namespace UniGame.ViewSystem.Runtime
{
    [Serializable]
    public class ViewModelTypeMap : IViewModelTypeMap
    {
        #region inspector
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
#endif
        public ViewReferencesMap viewsTypeMap = new ViewReferencesMap();

        #endregion

        public Type GetModelType(string viewType)
        {
            var view = viewsTypeMap.Find(viewType)
                .FirstOrDefault();
            
            return  view == null 
                ? ViewReferencesMap.Empty.ModelType
                : view.ModelType;
        }
        
        public Type GetViewModelType(string viewType)
        {
            var view = viewsTypeMap.Find(viewType)
                .FirstOrDefault();
            
            return  view == null 
                ? ViewReferencesMap.Empty.ViewModelType
                : view.ViewModelType;
        }

        public UiViewReference FindView(string id, string skinTag = "", string viewName = "")
        {
            var items = FindViews(id);
            return items.SelectReference(skinTag, viewName);
        }

        public IReadOnlyList<UiViewReference> FindViews(string id) => viewsTypeMap.Find(id);
        
        public Type GetViewType(string id)
        {
            var viewType =  viewsTypeMap
                .Find(id)
                .Select(x => x.Type)
                .FirstOrDefault();
            
            return viewType;
        }
        
        public void RegisterViewReference(IEnumerable<UiViewReference> sourceViews)
        {
            foreach (var view in sourceViews)
            {
                RegisterViewReference(view);
            }
        }

        public void RegisterViewReference(UiViewReference viewReference)
        {
            viewsTypeMap.Add(viewReference);
        }

        
    }
}