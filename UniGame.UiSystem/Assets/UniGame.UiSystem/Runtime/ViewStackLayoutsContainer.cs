
namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime;

    public class ViewStackLayoutsContainer : IViewLayoutContainer
    {

        private IDictionary<ViewType, IViewLayoutController> _viewControllers;

        private IViewLayoutController _dummyController;


        public ViewStackLayoutsContainer(IDictionary<ViewType, IViewLayoutController> layoutMap)
        {
            _viewControllers = layoutMap;
            //empty object controller
            _dummyController = new ViewStackController();
        }


        public IReadOnlyViewLayoutController this[ViewType type] => GetViewController(type);

        public IEnumerable<IViewLayoutController> Controllers => _viewControllers.Values;

        public IViewLayoutController GetViewController(ViewType type)
        {
            return _viewControllers.TryGetValue(type, out var controller) ? 
                controller : 
                _dummyController;
        }

    }
}