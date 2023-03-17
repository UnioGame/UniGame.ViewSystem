using System;
using System.Collections.Generic;
using System.Linq;
using UniGame.UiSystem.Runtime.Settings;
using UniGame.ViewSystem.Runtime.Abstract;
using UniModules.UniGame.ViewSystem.Runtime.Extensions;
using UnityEngine;

namespace UniGame.ViewSystem.Runtime
{
    using Abstract;

    [Serializable]
    public class ViewModelTypeMap : IViewModelTypeMap
    {
        #region inspector
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
#endif
        public TypeViewReferenceDictionary viewsTypeMap = new TypeViewReferenceDictionary(16);
        
        [Space]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
#endif
        public TypeViewReferenceDictionary modelTypeMap = new TypeViewReferenceDictionary(16);
        
        #endregion

        private Dictionary<Type, Type> modelTypeFromViewType = new Dictionary<Type, Type>(64);
        private Dictionary<Type, Type> modelViewTypeFromViewType = new Dictionary<Type, Type>(64);

        public Type GetModelTypeByView(Type viewType, bool strongTypeMatching = true)
        {
            var modelType =  viewsTypeMap
                .FindByType(viewType,strongTypeMatching)
                .Select(x => x.ModelType)
                .FirstOrDefault();
            
            return modelType;
        }
        
        public Type GetViewModelTypeByView(Type viewType, bool strongTypeMatching = true)
        {
            var modelType =  viewsTypeMap
                .FindByType(viewType,strongTypeMatching)
                .Select(x => x.ViewModelType)
                .FirstOrDefault();
            
            return modelType;
        }

        public UiViewReference FindView(
            Type viewType,
            string skinTag = "", 
            string viewName = "",
            bool strongMatching = true)
        {
            var views = FindViewsByType(viewType, strongMatching);
            var item = views.SelectReference(skinTag,viewName);
            return item;
        }
        
        public IReadOnlyList<UiViewReference> FindViewsByType(Type viewType, bool strongMatching = true) =>
            viewsTypeMap.FindByType(viewType, strongMatching);

        public IReadOnlyList<UiViewReference> FindModelByType(Type modelType, bool strongMatching = true) =>
            modelTypeMap.FindByType(modelType, strongMatching);
        
        public Type GetViewTypeByModel(Type modeType, bool strongTypeMatching = true)
        {
            var viewType =  modelTypeMap
                .FindByType(modeType,strongTypeMatching)
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
            RegisterViews(viewReference, viewReference.Type, viewsTypeMap);
            RegisterViews(viewReference, viewReference.ModelType, modelTypeMap);
        }

        private void RegisterViews(UiViewReference reference,Type targetType, TypeViewReferenceDictionary map)
        {
            if (map.TryGetValue(targetType, out var items) == false)
            {
                items = new UiReferenceList();
                map[targetType] = items;
            }

            if (items.references.Contains(reference))
                return;
            
            items.references.Add(reference);
        }
        
    }
}