
namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime;

    public class ViewStackLayoutsContainer : IViewLayoutContainer
    {

        private IDictionary<ViewType, IViewLayout> _viewControllers;

        private IViewLayout _dummyController;


        public ViewStackLayoutsContainer(IDictionary<ViewType, IViewLayout> layoutMap)
        {
            _viewControllers = layoutMap;
            //empty object controller
            _dummyController = new ViewLayout();
        }


        public IReadOnlyViewLayout this[ViewType type] => GetViewController(type);

        public IEnumerable<IViewLayout> Controllers => _viewControllers.Values;

        public IViewLayout GetViewController(ViewType type)
        {
            return _viewControllers.TryGetValue(type, out var controller) ? 
                controller : 
                _dummyController;
        }

    }
}