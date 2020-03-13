namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UniRx;
    using UnityEngine;

    public class ScreenViewStackController : IViewStackController<ITrackableView>, IDisposable, ILifeTimeContext, IViewStackController
    {
        private LifeTimeDefinition lifeTimeDefinition;

        private List<ITrackableView> _viewStack;

        private ReactiveCommand<IView> topChanged;

        public IObservable<IView> StackTopChanged => topChanged;

        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        public Transform Layout { get; private set; }

        public ScreenViewStackController(Transform layout)
        {
            lifeTimeDefinition = ClassPool.Spawn<LifeTimeDefinition>();
            topChanged         = new ReactiveCommand<IView>();
            topChanged.AddTo(lifeTimeDefinition.LifeTime);
            _viewStack  = new List<ITrackableView>();
            this.Layout = layout;
        }

        public void Push(ITrackableView view)
        {
            // HACK
            //(view as Component).transform.SetParent(Layout);

            view.OnShow.Subscribe(OnViewShow).AddTo(LifeTime);
            view.OnHide.Subscribe(OnViewHide).AddTo(LifeTime);
            view.OnClosed.Subscribe(OnViewClosed).AddTo(LifeTime);

            _viewStack.Add(view);
            for (var i = 0; i < _viewStack.Count - 1; i++)
                _viewStack[i].Hide();
            NotifyTopChanged();
        }

        public T Get<T>() where T : Component, IView
        {
            return (T)_viewStack.Find(v => v is T);
        }


        public void CloseAll()
        {
            _viewStack.ForEach(v=>v.Close());
        }

        public void Dispose()
        {
            lifeTimeDefinition.Release();
        }

        public bool Contains(IView view) => _viewStack.IndexOf((ITrackableView)view) > 0;

        public void Add<TView>(TView view) where TView : Component, IView
        {
            var trackable = view as ITrackableView;
            if (trackable != null)
                Push(trackable);
            trackable.Show();
        }

        private void OnViewShow(ITrackableView view)
        {
            var viewIndex = _viewStack.IndexOf(view);
            if (viewIndex < 0 || viewIndex == _viewStack.Count - 1)
                return;
            _viewStack.RemoveAt(viewIndex);
            _viewStack.Add(view);
            _viewStack[_viewStack.Count - 2].Hide();
            NotifyTopChanged();
        }

        private void OnViewHide(ITrackableView view)
        {
            var viewIndex = _viewStack.IndexOf(view);
            if (viewIndex < 0 || _viewStack.Count <= 1 || viewIndex != _viewStack.Count - 1)
                return;
            _viewStack.RemoveAt(viewIndex);
            _viewStack.Insert(viewIndex - 1, view);
            _viewStack[viewIndex].Show();
            NotifyTopChanged();
        }

        private void OnViewClosed(ITrackableView view)
        {
            OnViewHide(view);
            _viewStack.Remove(view);
            NotifyTopChanged();
        }

        private void NotifyTopChanged()
        {
            topChanged.Execute(_viewStack.Count > 0 ? _viewStack[_viewStack.Count - 1] : null);
        }

    }
}