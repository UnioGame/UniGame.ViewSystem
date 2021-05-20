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
                ? models.ToList()
                : models.Where(x => x.ViewModelType == modelType).ToList();

            if (!string.IsNullOrEmpty(skinTag) && !string.IsNullOrEmpty(viewName) && modelType != null)
            {
                return resultModel.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(skinTag) && resultModel.FirstOrDefault(x => string.IsNullOrEmpty(x.Tag)) != null)
            {
                resultModel = resultModel.Where(x => string.IsNullOrEmpty(x.Tag)).ToList();
            }

            if (string.IsNullOrEmpty(viewName) && resultModel.FirstOrDefault(x => string.IsNullOrEmpty(x.ViewName)) != null)
            {
                resultModel = resultModel.Where(x => string.IsNullOrEmpty(x.ViewName)).ToList();
            }

            if (modelType == null && resultModel.FirstOrDefault(x => x.Type == null) != null)
            {
                resultModel = resultModel.Where(x => x.Type == null).ToList();
            }

            return resultModel.FirstOrDefault();
        }
    
    }
}
