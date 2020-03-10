using UnityEngine;

namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstracts;
    using Settings;
    using Taktika.Addressables.Reactive;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniCore.Runtime.ProfilerTools;
    using Object = UnityEngine.Object;

    public class UiResourceProvider : IViewResourceProvider
    {
        private Dictionary<Type,List<UiViewDescription>> views = new Dictionary<Type, List<UiViewDescription>>(32);

        public IAddressableObservable<TView> LoadViewAsync<TView>(bool strongMatching = true) 
            where TView : Object
        {
            return LoadViewAsync<TView>(string.Empty,strongMatching);
        }
        
        public IAddressableObservable<TView> LoadViewAsync<TView>(string skin,bool strongMatching = true) 
            where TView : Object
        {
            var items = FindItemsByType(typeof(TView), strongMatching);
            
            var item = items.FirstOrDefault(
                x => string.IsNullOrEmpty(skin) || 
                     string.Equals(x.Tag,skin,StringComparison.InvariantCultureIgnoreCase));
            
            //return collection to pool
            items.DespawnCollection();

            if (item == null) {
                Debug.LogError($"{nameof(UiResourceProvider)} ITEM MISSING skin:{skin} type {typeof(TView).Name}");
                return null;
            }
            
            return item.View.ToObservable<TView>();
        }

        public List<IAddressableObservable<TView>> LoadViewsAsync<TView>(bool strongMatching = true) 
            where TView : Object
        {
            return LoadViewsAsync<TView>(string.Empty, strongMatching);
        }

        public List<IAddressableObservable<TView>> LoadViewsAsync<TView>(string skinTag, bool strongMatching = true) 
            where TView : Object
        {
            var result = new List<IAddressableObservable<TView>>();

            var items = FindItemsByType(typeof(TView), strongMatching);
            //return collection to pool
            items.DespawnCollection();

            if (items.Count <= 0) {
                Debug.LogError($"{nameof(UiResourceProvider)} ITEM MISSING skin:{skinTag} type {typeof(TView).Name}");
                return null;
            }

            foreach (var item in items) {
                result.Add(item.View.ToObservable<TView>());
            }
            
            return result;
        }

        
        public void RegisterViews(IReadOnlyList<UiViewDescription> sourceViews)
        {
            foreach (var view in sourceViews) {
                Type targetType = view.Type;
                if (views.TryGetValue(targetType, out var items) == false) {
                    items             = new List<UiViewDescription>();
                    views[targetType] = items;
                }
                items.Add(view);
            }
        }


        private List<UiViewDescription> FindItemsByType(Type type, bool strongMatching)
        {
            var result = ClassPool.Spawn<List<UiViewDescription>>();
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
