using System;
using System.Collections.Generic;
using System.Linq;
using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem.Runtime.Extensions
{
    public static class ViewOperationExtensions 
    {

        public static UiViewReference SelectReference
        (
            this IReadOnlyList<UiViewReference> items, 
            string skinTag = "",
            string viewName = "")
        {
            var item = items.FirstOrDefault(x => 
                (string.IsNullOrEmpty(skinTag) || 
                 string.Equals(x.Tag, skinTag, StringComparison.InvariantCultureIgnoreCase)) && 
                (string.IsNullOrEmpty(viewName) || string.Equals(x.ViewName, viewName, StringComparison.InvariantCultureIgnoreCase)));
            return item;
        }
    
    }
}
