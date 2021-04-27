using System;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    public interface IViewModelTypeMap
    {
        Type GetModelTypeByView(Type viewType, bool strongTypeMatching = true);
        Type GetViewTypeByModel(Type modeType, bool strongTypeMatching = true);
    }
}