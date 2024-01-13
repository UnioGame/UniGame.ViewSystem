using System;
using System.Collections.Generic;
using System.Linq;
using UniModules.UniCore.Runtime.ReflectionUtils;
using UniModules.UniCore.Runtime.Utils;
using UniGame.Core.Runtime.SerializableType;
using UniGame.UiSystem.Runtime;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

namespace UniModules.UniGame.ViewSystem
{
    public static class ViewSystemUtils
    {
        public static readonly List<Type> Empty = new List<Type>(0);
        
        public static readonly MemorizeItem<Type, IReadOnlyList<Type>> ModelsRealizationMap =
            MemorizeTool.Memorize<Type, IReadOnlyList<Type>>(type =>
            {
                if (!type.IsAbstract && !type.IsInterface)
                    return Empty;
                
                return type.GetAssignableTypes()
                    .Where(x => x!=null)
                    .Select(x => x)
                    .ToList();
            });

        public static readonly MemorizeItem<Type, Type> CachedModelTypes =
            MemorizeTool.Memorize<Type, Type>(GetModelTypeByViewNonCached);

        public static IReadOnlyList<Type> GetValue(Type type)
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

#if ODIN_INSPECTOR
        public static IEnumerable<ValueDropdownItem<SType>> GetModelSTypeVariants(Type type)
        {
            foreach (var target in GetTypesVariants(type))
            {
                yield return new ValueDropdownItem<SType>()
                {
                    Text = target == null ? String.Empty : target.Name,
                    Value = (SType)target,
                };
            }
        }

        public static IEnumerable<ValueDropdownItem<Type>> GetModelVariants(Type modelType)
        {
            foreach (var target in GetTypesVariants(modelType))
            {
                yield return new ValueDropdownItem<Type>()
                {
                    Text = target == null ? String.Empty : target.Name,
                    Value = target,
                };
            }
        }
#endif
        
        public static IEnumerable<Type> GetTypesVariants(Type modelType)
        {
            var type = modelType;
            if (type != null && !type.IsAbstract && !type.IsInterface)
            {
                yield return type;
            }

            var items = GetValue(type);
            foreach (var item in items)
            {
                if(item == type) continue;
                
                if (item == null || item.IsAbstract || item.IsInterface)
                    continue;
                
                yield return type;
            }
        }

        
        public static Type GetViewModelType(Type viewType)
        {
            var modelType = ViewSystemUtils.GetModelTypeByView(viewType);
            var viewModelType = ViewSystemUtils.GetFirstAssignable(modelType);
            return viewModelType;
        }
        
        public static Type GetModelTypeByView(Type viewType)
        {
            return CachedModelTypes[viewType];
        }
        
        public static Type GetModelTypeByViewNonCached(Type viewType)
        {
            var viewInterface = viewType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == ViewSystemConstants.BaseViewType);

            if (viewInterface == null)
                return typeof(ViewModelBase);
            
            var modelsArgs = viewInterface.GetGenericArguments();
            var modelType = modelsArgs.FirstOrDefault();
            return modelType;
        }
    }
}