namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;

    public abstract class UiContainerView<TViewModel> : UiCanvasGroupView<TViewModel>, IUiContainer where TViewModel : class, IViewModel
    {
        private readonly IList<ViewBase> _childViews = new List<ViewBase>();
        
        public void Add(IView view)
        {
            if (view is ViewBase monoView) {
                if (!_childViews.Contains(monoView)) {
                    _childViews.Add(monoView);

                    monoView.transform.SetParent(transform, false);
                    monoView.OnClosed.Subscribe(Remove).AddTo(LifeTime);
                }
            }
        }

        public void Remove(IView view)
        {
            if (view is ViewBase monoView) {
                if (_childViews.Contains(monoView)) {
                    _childViews.Remove(monoView);
                    monoView.transform.SetParent(null);
                }
            }
        }
    }
}