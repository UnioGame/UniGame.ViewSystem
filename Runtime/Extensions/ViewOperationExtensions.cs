using System;
using System.Collections.Generic;
using System.Linq;
using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem.Runtime.Extensions
{
    public static class ViewOperationExtensions 
    {

        public static UiViewReference SelectReference(
            this IEnumerable<UiViewReference> items, 
            string skinTag = "",
            string viewName = "",
            Type modelType = null)
        {
            var models = string.IsNullOrEmpty(skinTag)
                ? items
                : items.Where(x => string.Equals(x.Tag, skinTag, StringComparison.InvariantCultureIgnoreCase));

            models = string.IsNullOrEmpty(viewName)
                ? models
                : models.Where(x => string.Equals(x.ViewName, viewName, StringComparison.InvariantCultureIgnoreCase));

            
            var resultModel = modelType == null
                ? models
                : models.Where(x => x.ViewModelType == modelType);

            var result = resultModel.FirstOrDefault();

            return result;
        }
    
    }
}
