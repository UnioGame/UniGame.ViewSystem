using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using Core.Runtime;
    using UniModules.UniGame.UISystem.Runtime;
    using ViewSystem.Runtime;
    using UniRx;

    public sealed class DummyView : IView
    {
        private readonly ReactiveProperty<ViewStatus> _viewStatusProperty = new ReactiveProperty<ViewStatus>(ViewStatus.None);
        private readonly ReactiveProperty<bool> _isVisibleProperty = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<bool> _isInitializedProperty = new ReactiveProperty<bool>(false);

        private static EmptyViewModel emptyViewModel = new EmptyViewModel();
        private static DummyView      instance;

        public ILifeTime LifeTime => UniModules.UniCore.Runtime.DataFlow.LifeTime.TerminatedLifetime;

        public IReadOnlyReactiveProperty<ViewStatus> Status => _viewStatusProperty;

        public Type ModelType => typeof(IViewModel);
        
        public IViewsLayout Layout => GameViewSystem.ViewSystem;

        public GameObject Owner     => null;
        public GameObject GameObject => null;
        public Transform  Transform => null;
        
        public ILifeTime ModelLifeTime => UniModules.UniCore.Runtime.DataFlow.LifeTime.TerminatedLifetime;
        public ILifeTime ViewLifeTime  => UniModules.UniCore.Runtime.DataFlow.LifeTime.TerminatedLifetime;

        public int ViewIdHash => 0;
        public IReadOnlyReactiveProperty<bool> IsVisible     => _isVisibleProperty;
        public IReadOnlyReactiveProperty<bool> IsInitialized => _isInitializedProperty;
        public IViewModel                      ViewModel     => emptyViewModel;
        public IObservable<IViewModel> OnViewModelChanged => Observable.Empty<IViewModel>();
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

        public IObservable<IView> SelectStatus(ViewStatus status)
        {
            return _viewStatusProperty.Where(x => x == status).Select(x => this);
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