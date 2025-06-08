using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using Core.Runtime;
    using R3;
    using ViewSystem.Runtime;
     

    public sealed class DummyView : IView
    {
        private readonly ReactiveProperty<ViewStatus> _viewStatusProperty = new ReactiveProperty<ViewStatus>(ViewStatus.None);
        private readonly ReactiveProperty<bool> _isVisibleProperty = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<bool> _isInitializedProperty = new ReactiveProperty<bool>(false);

        private static EmptyViewModel emptyViewModel = new EmptyViewModel();
        private static DummyView      instance;

        public ILifeTime LifeTime => UniGame.Runtime.DataFlow.LifeTime.TerminatedLifetime;

        public ReadOnlyReactiveProperty<ViewStatus> Status => _viewStatusProperty;

        public Type ModelType => typeof(IViewModel);
        
        public IViewsLayout Layout => GameViewSystem.ViewSystem;

        public GameObject Owner     => null;
        public GameObject GameObject => null;
        public Transform  Transform => null;
        
        public ILifeTime ModelLifeTime => UniGame.Runtime.DataFlow.LifeTime.TerminatedLifetime;
        public ILifeTime ViewLifeTime  => UniGame.Runtime.DataFlow.LifeTime.TerminatedLifetime;

        public int ViewIdHash => 0;
        public ReadOnlyReactiveProperty<bool> IsVisible     => _isVisibleProperty;
        public ReadOnlyReactiveProperty<bool> IsInitialized => _isInitializedProperty;
        public IViewModel                      ViewModel     => emptyViewModel;
        public Observable<IViewModel> OnViewModelChanged => Observable.Empty<IViewModel>();
        public string SourceName { get; set; }
        public string ViewId { get; set; }

        public IView BindToView<T>(IObservable<T> source, Action<T> action, int frameThrottle = 0) => this;

        public bool IsTerminated => LifeTime.IsTerminated;
        public bool IsModelAttached => true;

        private DummyView()
        {
            // close constructor
        }

        public static IView Create()
        {
            return instance ??= new DummyView();
        }

        public void Destroy()
        {
           // do nothing
        }

        public void Close()
        {
            // do nothing
        }

        public IView Show() => this;
        public async UniTask<IView> ShowAsync() => Show();

        public void Hide() { }

        public UniTask CloseAsync()
        {
            Close();
            return UniTask.CompletedTask;
        }

        public Observable<IView> SelectStatus(ViewStatus status)
        {
            return _viewStatusProperty
                .Where(x => x == status)
                .Select(x => this as IView);
        }

        public void SetSourceName(string viewId, string sourceName)
        {
            ViewId = viewId;
            SourceName = sourceName;
        }

        public UniTask<IView> Initialize(IViewModel vm, bool isViewOwner = false)
        {
            // do nothing
            return UniTask.FromResult<IView>(this);
        }
    }
}