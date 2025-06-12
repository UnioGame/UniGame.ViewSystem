using System;
using System.Collections.Generic;
using System.Linq;
using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem.Runtime.Extensions
{
    public static class ViewOperationExtensions 
    {
        private static readonly UiViewReference Empty = new()
        {
            ViewName = "EMPTY",
            ModelType = typeof(EmptyViewModel),
            ViewModelType = typeof(EmptyViewModel),
        };
        
        private static List<UiViewReference> _cachedList = new(16);

        public static UiViewReference SelectReference(
            this IEnumerable<UiViewReference> source, 
            string skinTag = "",
            string viewName = "",
            Type modelType = null)
        {
            _cachedList.Clear();
            
            var isEmptyTag = string.IsNullOrEmpty(skinTag);
            var isEmptyName = string.IsNullOrEmpty(viewName);
            var isEmptyModelType = modelType == null;

            foreach (var viewReference in source)
            {
                if(isEmptyTag && !string.IsNullOrEmpty(viewReference.Tag))
                    continue;
                
                if (!isEmptyTag && !string.Equals(viewReference.Tag, skinTag, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                
                if (!isEmptyName && !string.Equals(viewReference.ViewName, viewName, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                
                if (!isEmptyModelType && viewReference.ViewModelType != modelType)
                    continue;

                _cachedList.Add(viewReference);
            }

            var result = _cachedList.FirstOrDefault();

            _cachedList.Clear();
            return result;
        }
    
    }
}
