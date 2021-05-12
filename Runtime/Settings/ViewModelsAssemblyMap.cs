using System;
using System.Collections.Generic;
using System.Linq;
using UniModules.UniCore.Runtime.ReflectionUtils;
using UniModules.UniCore.Runtime.Utils;
using UniModules.UniGame.Core.Runtime.SerializableType;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{
    public static class ViewModelsAssemblyMap
    {
        public static readonly List<SType> Empty = new List<SType>(0);
        
        public static readonly MemorizeItem<Type, IReadOnlyList<SType>> ModelsRealizationMap =
            MemorizeTool.Memorize<Type, IReadOnlyList<SType>>(type =>
            {
                if (!type.IsAbstract && !type.IsInterface)
                    return Empty;
                
                return type.GetAssignableTypes()
                    .Where(x => x!=null)
                    .Select(x => (SType)x)
                    .ToList();
            });

        public static IReadOnlyList<SType> GetValue(Type type)
        {
            return ModelsRealizationMap[type];
        }

        public static Type GetFirstAssignable(Type type)
        {
            if (!type.IsAbstract && !type.IsInterface)
                return type;
            var items = GetValue(type);
            return items.FirstOrDefault();
        }
        
    }
}