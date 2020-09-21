namespace UniGame.UiSystem.ModelViews.Runtime.Flow
{
    using System;
    using UiSystem.Runtime.Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.Core.Runtime.Rx;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime;
    using UniRx;

    /// <summary>
    /// Model View handle
    /// </summary>
    public class ViewHandle : IViewHandle
    {
        private LifeTimeDefinition _lifeTime     = new LifeTimeDefinition();
        private LifeTimeDefinition _viewLifeTime = new LifeTimeDefinition();

        private RecycleReactiveProperty<IViewHandle> _observable = new RecycleReactiveProperty<IViewHandle>();
        private RecycleReactiveProperty<ViewStatus>  _rxStatus     = new RecycleReactiveProperty<ViewStatus>();
        private IView _view;
        private ReactiveCommand _notifyCommand = new ReactiveCommand();

        private IViewModel _model;
        private Type       _modelType;
        private Type       _viewType;

        #region public properties

        public IReadOnlyReactiveProperty<ViewStatus> Status => _rxStatus;
        

        public IView View => _view;

        public IViewModel Model     => _model;
        public Type       ModelType => _modelType;

        public Type ViewType {
            get => _viewType;
            set => _viewType = value;
        }

        public ILifeTime LifeTime => _lifeTime;

        #endregion

        #region constructor

        public ViewHandle(IViewModel model, Type modelType, Type viewType)
        {
            _model     = model;
            _modelType = modelType;
            _viewType  = viewType;

            _lifeTime.AddDispose(_observable);
            _lifeTime.AddDispose(_rxStatus);

            _lifeTime.AddCleanUpAction(CleanUp);
            _lifeTime.AddCleanUpAction(_viewLifeTime.Release);

            _rxStatus.Subscribe(x => _notifyCommand.Execute()).
                AddTo(_lifeTime);

            _notifyCommand.
                Subscribe(x => Notify()).
                AddTo(_lifeTime);
            
            _observable.Value = this;
        }

        #endregion

        #region public methods

        public void Notify()
        {
            _observable.SetValueForce(this);
        }

        public void SetView(IView view)
        {
            if (_lifeTime.IsTerminated)
                return;

            UpdateViewEvents(view);
        }

        public void Dispose()
        {
            _lifeTime.Terminate();
        }

        public IDisposable Subscribe(IObserver<IViewHandle> observer) =>
            _observable.Subscribe(observer);

        #endregion

        #region view commands methods

        public void Destroy() => View?.Destroy();

        public void Close() => View?.Close();

        public void Show() => View?.Show();

        public void Hide() => View?.Hide();

        #endregion

        #region private methods

        private void CleanUp()
        {
            _model     = null;
            _viewType  = null;
            _modelType = null;
        }

        private void UpdateViewEvents(IView view)
        {
            if (view == View)
                return;

            _viewLifeTime.Release();
                                    
            _view = view;

            if (view == null)
                return;

            view.Status.
                Do(x => _rxStatus.Value = x).
                Where(x => x == ViewStatus.Closed).
                Do(x => _view = null).
                Do(x => _viewLifeTime.Release()).
                Subscribe().
                AddTo(_viewLifeTime);

            _rxStatus.SetValueForce(view.Status.Value);
        }

        #endregion
    }
}