using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstracts;
    using Addressables.Reactive;
    using Settings;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using Object = UnityEngine.Object;

    public class UiResourceProvider : IViewResourceProvider
    {
        private Dictionary<Type,List<UiViewReference>> views = new Dictionary<Type, List<UiViewReference>>(32);

        public IAddressableObservable<TView> LoadViewAsync<TView>(bool strongMatching = true) 
            where TView : Object
        {
            return LoadViewAsync<TView>(string.Empty,strongMatching);
        }
        
        public IAddressableObservable<TView> LoadViewAsync<TView>(Type viewType,string skin,bool strongMatching = true) 
            where TView : Object
        {
            var items = FindItemsByType(viewType, strongMatching);
            
            var item = items.FirstOrDefault(
                x => string.IsNullOrEmpty(skin) || 
                     string.Equals(x.Tag,skin,StringComparison.InvariantCultureIgnoreCase));
            
            //return collection to pool
            items.Despawn();

            if (item == null) {
                Debug.LogError($"{nameof(UiResourceProvider)} ITEM MISSING skin:{skin} type {typeof(TView).Name}");
                return null;
            }
            
            return item.View.ToObservable<TView>();
        }

        public IAddressableObservable<TView> LoadViewAsync<TView>(string skin, bool strongMatching = true)
            where TView : Object
        {
            return LoadViewAsync<TView>(typeof(TView), skin, strongMatching);
        }

        public List<IAddressableObservable<TView>> LoadViewsAsync<TView>(bool strongMatching = true) 
            where TView : Object
        {
            return LoadViewsAsync<TView>(string.Empty, strongMatching);
        }

        public List<IAddressableObservable<TView>> LoadViewsAsync<TView>(Type viewType,string skinTag, bool strongMatching = true)
            where TView : Object
        {
            var items = FindItemsByType(viewType, strongMatching);
            //return collection to pool
            items.Despawn();

            if (items.Count <= 0) {
                Debug.LogError($"{nameof(UiResourceProvider)} ITEM MISSING skin:{skinTag} type {typeof(TView).Name}");
                return null;
            }
            
            var result = new List<IAddressableObservable<TView>>();

            foreach (var item in items) {
                result.Add(item.View.ToObservable<TView>());
            }
            
            return result;
        }
        
        public List<IAddressableObservable<TView>> LoadViewsAsync<TView>(string skinTag, bool strongMatching = true) 
            where TView : Object
        {

            return LoadViewsAsync<TView>(typeof(TView), skinTag, strongMatching);

        }

        
        public void RegisterViews(IReadOnlyList<UiViewReference> sourceViews)
        {
            foreach (var view in sourceViews) {
                Type targetType = view.Type;
                if (views.TryGetValue(targetType, out var items) == false) {
                    items             = new List<UiViewReference>();
                    views[targetType] = items;
                }
                items.Add(view);
            }
        }


        private List<UiViewReference> FindItemsByType(Type type, bool strongMatching)
        {
            var result = ClassPool.Spawn<List<UiViewReference>>();
            if (strongMatching) {
                if (views.TryGetValue(type, out var items)) {
                    result.AddRange(items);
                }
                return result;
            }

            foreach (var view in views) {
                var viewType = view.Key;
                if(type.IsAssignableFrom(viewType))
                    result.AddRange(view.Value);
            }

            return result;
        }
    }
}
