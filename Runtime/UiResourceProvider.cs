using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;
using UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Abstract;
using UniModules.UniGame.ViewSystem.Runtime.Extensions;
using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Addressables.Reactive;
    using Settings;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using Object = UnityEngine.Object;

    public class UiResourceProvider : IViewResourceProvider<Component>
    {
        private IViewModelTypeMap _viewModelTypeMap;

        public UiResourceProvider(IViewModelTypeMap viewModelTypeMap)
        {
            _viewModelTypeMap = viewModelTypeMap;
        }
        
        public IAddressableObservable<Component> LoadViewAsync(Type viewType,
            string skinTag = "",
            bool strongMatching = true,
            string viewName = "")
        {
            var items = _viewModelTypeMap.FindViewsByType(viewType, strongMatching);
            var item = items.SelectReference(skinTag,viewName);

            if (item == null)
            {
                Debug.LogError($"{nameof(UiResourceProvider)} ITEM MISSING skin:{skinTag} type {viewType.Name}");
                return null;
            }

            return item.View.ToObservable<Component>();
        }

        public List<IAddressableObservable<Component>> LoadViewsAsync(Type viewType, string skinTag = null, bool strongMatching = true)
        {
            var items = _viewModelTypeMap.FindViewsByType(viewType, strongMatching);

            if (items.Count <= 0)
            {
                Debug.LogError($"{nameof(UiResourceProvider)} ITEM MISSING skin:{skinTag} type {viewType.Name}");
                return null;
            }

            var result = new List<IAddressableObservable<Component>>();

            foreach (var item in items)
            {
                result.Add(item.View.ToObservable<Component>());
            }

            return result;
        }


    }
}
