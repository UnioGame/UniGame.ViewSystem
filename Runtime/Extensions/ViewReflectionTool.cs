using System;
using UniModules.UniCore.Runtime.Utils;
using UniModules.UniGame.ViewSystem.Editor.UiEditor;

public static class ViewReflectionTool
{
    public static MemorizeItem<Type, Type> _baseViewType = MemorizeTool.Memorize<Type, Type>(type =>
    {
        var baseViewType  = ViewSystemConstants.BaseViewType;
        var interfaces = type.GetInterfaces();
        foreach (var interfaceValue in interfaces)
        {
            var genericType = interfaceValue.IsGenericType ?
                interfaceValue.GetGenericTypeDefinition() : null;
            if(genericType == null) continue;
            if (baseViewType == genericType)
                return interfaceValue;
        }
        
        return type;
    });

    public static Type GetBaseViewType(Type type)
    {
        return _baseViewType[type];
    }
    
}
