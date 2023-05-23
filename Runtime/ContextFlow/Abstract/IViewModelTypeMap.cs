using System;
using System.Collections.Generic;
using UniGame.UiSystem.Runtime.Settings;

namespace UniGame.ViewSystem.Runtime
{
    public interface IViewModelTypeMap
    {
        IReadOnlyList<UiViewReference> FindViews(string viewType);
        Type GetModelType(string viewType);
        Type GetViewModelType(string viewType);
        Type GetViewType(string modeType);
    }
}