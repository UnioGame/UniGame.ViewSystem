namespace UniGame.ViewSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using UiSystem.Runtime.Settings;

    public static class ViewTypeMapExtensions
    {
        public static IReadOnlyList<UiViewReference> FindViews(this IViewModelTypeMap map,Type viewType)
        {
            return map.FindViews(viewType.Name);
        }
        
        public static Type GetModelType(this IViewModelTypeMap map,Type viewType)
        {
            return map.GetModelType(viewType.Name);
        }
        
        public static Type GetViewModelType(this IViewModelTypeMap map,Type viewType)
        {
            return map.GetViewModelType(viewType.Name);
        }
        
        public static Type GetViewType(this IViewModelTypeMap map,Type viewType)
        {
            return map.GetViewType(viewType.Name);
        }

    }
}