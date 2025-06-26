namespace UniGame.UiSystem.Runtime
{
    using System;
    using UniGame.Runtime.DataFlow;
    using UniGame.Runtime.Rx;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using R3;
    using UniGame.Runtime.Rx.Extensions;
    using ViewSystem.Runtime;
     

    /// <summary>
    /// Model View handle
    /// </summary>
    public class ViewHandle : IViewHandle
    {
        private LifeTime _lifeTime     = new();
        private LifeTime _viewLifeTime = new();

        private ReactiveValue<IViewHandle> _observable = new();
        private ReactiveValue<ViewStatus>  _rxStatus     = new();
        private IView _view;
        private ReactiveCommand _notifyCommand = new();

        private IViewModel _model;
        private Type       _modelType;
        private Type       _viewType;

        #region public properties

        public ReadOnlyReactiveProperty<ViewStatus> Status => _rxStatus;
        

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

        public IView Show() => View?.Show();
        public async UniTask<IView> ShowAsync() => Show();

        public void Hide() => View?.Hide();
        
        public UniTask CloseAsync() => View?.CloseAsync() ?? UniTask.CompletedTask;

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

            _rxStatus.SetValueForce(view.Status.CurrentValue);
        }

        #endregion
    }
}