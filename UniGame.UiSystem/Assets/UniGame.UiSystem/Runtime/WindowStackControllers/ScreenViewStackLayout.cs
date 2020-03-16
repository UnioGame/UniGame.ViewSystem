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

    public class ScreenViewStackLayout : IDisposable, ILifeTimeContext, IViewLayout
    {
        private LifeTimeDefinition _lifeTimeDefinition;

        private List<IView> _viewStack;

        private ReactiveCommand<IView> topChanged;

        public IObservable<IView> StackTopChanged => topChanged;

        public ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        public Transform Layout { get; private set; }

        public ScreenViewStackLayout(Transform layout)
        {
            _lifeTimeDefinition = ClassPool.Spawn<LifeTimeDefinition>();
            topChanged         = new ReactiveCommand<IView>();
            topChanged.AddTo(_lifeTimeDefinition.LifeTime);
            _viewStack  = new List<IView>();
            this.Layout = layout;
        }

        public void Push(IView view)
        {
            // HACK
            //(view as Component).transform.SetParent(Layout);

            view.OnShown.Subscribe(OnViewShow).AddTo(LifeTime);
            view.OnHidden.Subscribe(OnViewHide).AddTo(LifeTime);
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
            Debug.Log("View Stack close all");
            for (var index = _viewStack.Count -1 ; index >= 0; index--) {
                var view = _viewStack[index];
                view.Close();
                _viewStack.RemoveAt(index);
            }
        }

        public void Dispose()
        {
            _lifeTimeDefinition.Release();
        }

        public bool Contains(IView view) => _viewStack.IndexOf(view) > 0;

        public void Push<TView>(TView view) where TView : Component, IView
        {
            if (view != null)
                Push(view);
            view?.Show();
        }

        private void OnViewShow(IView view)
        {
            var viewIndex = _viewStack.IndexOf(view);
            if (viewIndex < 0 || viewIndex == _viewStack.Count - 1)
                return;
            _viewStack.RemoveAt(viewIndex);
            _viewStack.Add(view);
            _viewStack[_viewStack.Count - 2].Hide();
            NotifyTopChanged();
        }

        private void OnViewHide(IView view)
        {
            var viewIndex = _viewStack.IndexOf(view);
            if (viewIndex < 0 || _viewStack.Count <= 1 || viewIndex != _viewStack.Count - 1)
                return;
            _viewStack.RemoveAt(viewIndex);
            _viewStack.Insert(viewIndex - 1, view);
            _viewStack[viewIndex].Show();
            NotifyTopChanged();
        }

        private void OnViewClosed(IView view)
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