
using UniModules.UniCore.Runtime.Utils;

namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using UniModules.UniGame.UiSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Abstract;

    public class ViewStackLayoutsContainer : IViewLayoutContainer
    {
        private IDictionary<string, IViewLayout> _viewControllers;

        private IViewLayout _dummyController;


        public ViewStackLayoutsContainer(IDictionary<ViewType, IViewLayout> layoutMap)
        {
            _viewControllers = new Dictionary<string, IViewLayout>(layoutMap.Count);
            foreach (var layoutItem in layoutMap)
            {
                _viewControllers[layoutItem.Key.ToStringFromCache()] = layoutItem.Value;
            }
            //empty object controller
            _dummyController = new ViewLayout();
        }
        
        public IReadOnlyViewLayout this[ViewType type] => GetLayout(type);
        
        public TView Get<TView>()  where TView : class, IView
        {
            foreach (var viewLayout in _viewControllers) {
                var layout = viewLayout.Value;
                var view = layout.Get<TView>();
                if (view!=null) return view;
            }

            return null;
        }

        public bool RegisterLayout(string id, IViewLayout layout)
        {
            if (HasLayout(id)) return false;
            _viewControllers[id] = layout;
            return true;
        }

        public bool HasLayout(string id) => GetLayout(id) != null;
        
        public IEnumerable<IViewLayout> Controllers => _viewControllers.Values;

        public IViewLayout GetLayout(ViewType type)
        {
            return GetLayout(type.ToStringFromCache());
        }

        public IViewLayout GetLayout(string id)
        {
            return _viewControllers.TryGetValue(id, out var controller) 
                ? controller 
                : _dummyController;
        }
    }
}