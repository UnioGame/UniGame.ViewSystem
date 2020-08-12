using UnityEngine;

namespace UniGame.UiSystem.Runtime.WindowStackControllers
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using Backgrounds.Abstract;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime;
    using UniRx;

    public enum LayoutType
    {
        Default,
        Stack
    }

    public class ViewsLayoutAsset : MonoBehaviour, IViewLayout
    {
        #region inspector

        [SerializeField]
        private Canvas _layoutCanvas;

        [SerializeField]
        private BackgroundFactory _backgroundFactory;

        [SerializeField]
        private LayoutType _layoutType = LayoutType.Stack;

        #endregion

        private readonly Lazy<IViewLayout> _layout;

        public IViewLayout LayoutController => _layout.Value;

        public Transform Layout => LayoutController.Layout;

        public IReadOnlyReactiveProperty<ViewStatus> Status => LayoutController.Status;
        
        public IObservable<IView> OnHidden => LayoutController.OnHidden;
        
        public IObservable<IView> OnShown => LayoutController.OnShown;
        
        public IObservable<IView> OnClosed  => LayoutController.OnClosed;
        
        public ILifeTime LifeTime => LayoutController.LifeTime;
        
        #region public methods

        public ViewsLayoutAsset()
        {
            _layout = new Lazy<IViewLayout>(Create);
        }

        public void Dispose() => LayoutController.Dispose();

        public bool Contains(IView view) => LayoutController.Contains(view);

        public void HideAll()
        {
            LayoutController.HideAll();
        }

        public void Push(IView view)
        {
            LayoutController.Push(view);
        }

        public TView Get<TView>() where TView : class,IView
        {
            return LayoutController.Get<TView>();
        }

        public List<TView> GetAll<TView>() where TView : class, IView
        {
            return LayoutController.GetAll<TView>();
        }

        public void CloseAll() => LayoutController.CloseAll();

        public void ShowLast() => LayoutController.ShowLast();

        #endregion


        #region private methods

        protected virtual IViewLayout Create()
        {
            IBackgroundView backgroundView = null;
            if (_backgroundFactory != null) {
                backgroundView = _backgroundFactory.Create();
            }

            return GetViewLayout(_layoutType, backgroundView);
        }

        protected void OnDestroy()
        {
            if(_layout.IsValueCreated)
                _layout.Value.Dispose();
        }

        private IViewLayout GetViewLayout(LayoutType layoutType, IBackgroundView backgroundView)
        {
            switch (layoutType) {
                case LayoutType.Stack:
                    return new StackViewLayout(_layoutCanvas.transform, backgroundView);
                default:
                    return new DefaultViewLayout(_layoutCanvas.transform, backgroundView);
            }
        }

        #endregion
    }
}