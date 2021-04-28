using System;
using System.Collections.Generic;
using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Abstract
{
    public interface IViewModelTypeMap
    {
        IReadOnlyList<UiViewReference> FindViewsByType(Type viewType, bool strongMatching = true);

        IReadOnlyList<UiViewReference> FindModelByType(Type modelType, bool strongMatching = true);

        Type GetModelTypeByView(Type viewType, bool strongTypeMatching = true);
        Type GetViewTypeByModel(Type modeType, bool strongTypeMatching = true);
    }
}