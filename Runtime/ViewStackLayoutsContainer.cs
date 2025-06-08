
using UniGame.Runtime.Utils;

namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using UniModules.UniGame.UiSystem.Runtime;
    using ViewSystem.Runtime;

    public class ViewStackLayoutsContainer : IViewLayoutContainer
    {
        private IDictionary<string, IViewLayout> _layouts;

        private IViewLayout _dummyController;
        
        public ViewStackLayoutsContainer(IDictionary<string, IViewLayout> layoutMap)
        {
            _layouts = new Dictionary<string, IViewLayout>(layoutMap.Count);
            
            foreach (var layoutItem in layoutMap)
                RegisterLayout(layoutItem.Key, layoutItem.Value);
            
            //empty object controller
            _dummyController = new ViewLayout();
        }
        
        public IReadOnlyViewLayout this[ViewType type] => GetLayout(type);
        
        public TView Get<TView>()  where TView : class, IView
        {
            foreach (var viewLayout in _layouts) {
                var layout = viewLayout.Value;
                var view = layout.Get<TView>();
                if (view!=null) return view;
            }

            return null;
        }

        public bool RegisterLayout(string id, IViewLayout layout)
        {
            if (_layouts.ContainsKey(id)) return false;
            _layouts[id] = layout;
            
            layout.LifeTime
                .AddCleanUpAction(() => RemoveLayout(layout));
            
            return true;
        }

        public bool HasLayout(string id)
        {
            return GetLayout(id) != _dummyController;
        }
        
        public IEnumerable<IViewLayout> Controllers => _layouts.Values;

        public bool RemoveLayout(IViewLayout layout)
        {
            var targetId = default(string);
            foreach (var pair in _layouts)
            {
                if(pair.Value != layout)continue;
                targetId = pair.Key;
                break;
            }
            if (string.IsNullOrEmpty(targetId)) return false;
            return RemoveLayout(targetId);
        }
        
        public bool RemoveLayout(string id)
        {
            if (!_layouts.TryGetValue(id, out var layout))
                return false;
            
            layout.Dispose();
            _layouts.Remove(id);
            return true;
        }

        public IViewLayout GetLayout(ViewType type)
        {
            return GetLayout(type.ToStringFromCache());
        }

        public IViewLayout GetLayout(string id)
        {
            return _layouts.TryGetValue(id, out var controller) 
                ? controller 
                : _dummyController;
        }

        public void CloseAll()
        {
            foreach (var layout in _layouts)
            {
                layout.Value.CloseAll();
            }
        }

        public void CloseAll(string id)
        {
            var layout = GetLayout(id);
            layout.CloseAll();
        }

        public void Dispose()
        {
            CloseAll();
        }
    }
}