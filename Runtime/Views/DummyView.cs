namespace UniGame.UiSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;

    public sealed class DummyView : IView
    {
        private readonly ReactiveProperty<ViewStatus> _viewStatusProperty = new ReactiveProperty<ViewStatus>(ViewStatus.None);
        private readonly ReactiveProperty<bool> _isVisibleProperty = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<bool> _isInitializedProperty = new ReactiveProperty<bool>(false);

        private static DummyView instance;

        public ILifeTime LifeTime => UniModules.UniCore.Runtime.DataFlow.LifeTime.TerminatedLifetime;

        public IReadOnlyReactiveProperty<ViewStatus> Status => _viewStatusProperty;

        public IReadOnlyReactiveProperty<bool> IsVisible     => _isVisibleProperty;
        public IReadOnlyReactiveProperty<bool> IsInitialized => _isInitializedProperty;

        public bool IsTerminated => LifeTime.IsTerminated;

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

        public void Show()
        {
            // do nothing
        }

        public void Hide()
        {
            // do nothing
        }

        public IObservable<IView> SelectStatus(ViewStatus status)
        {
            return _viewStatusProperty.Where(x => x == status).Select(x => this);
        }

        public UniTask Initialize(IViewModel vm, bool isViewOwner = false)
        {
            // do nothing
            
            return UniTask.CompletedTask;
        }
    }
}