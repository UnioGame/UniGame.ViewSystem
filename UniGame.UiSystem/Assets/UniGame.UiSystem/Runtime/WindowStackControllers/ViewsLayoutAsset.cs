using UnityEngine;

namespace UniGame.UiSystem.Runtime.WindowStackControllers
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;

    public class ViewsLayoutAsset : MonoBehaviour, IViewLayout
    {
        #region inspector

        public Canvas layoutCanvas;

        #endregion

        private Lazy<IViewLayout> layout;

        public IViewLayout LayoutController => layout.Value;

        public Transform Layout => LayoutController.Layout;

        public IObservable<IView> OnHidden => LayoutController.OnHidden;
        
        public IObservable<IView> OnShown => LayoutController.OnShown;
        
        public IObservable<IView> OnClosed  => LayoutController.OnClosed;
        
        public ILifeTime LifeTime => LayoutController.LifeTime;
        
        #region public methods

        public ViewsLayoutAsset()
        {
            layout = new Lazy<IViewLayout>(Create);
        }

        public void Dispose() => LayoutController.Dispose();

        public bool Contains(IView view) => LayoutController.Contains(view);

        public void HideAll()
        {
            LayoutController.HideAll();
        }

        public void Push<TView>(TView view) where TView : class,IView
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

        #endregion


        #region private methods

        protected virtual IViewLayout Create()
        {
            return new ViewsStackLayout(layoutCanvas.transform);
        }

        protected void OnDestroy()
        {
            if(layout.IsValueCreated)
                layout.Value.Dispose();
        }

        #endregion

    }
}