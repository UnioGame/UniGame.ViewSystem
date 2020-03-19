using UnityEngine;

namespace UniGame.UiSystem.Runtime.WindowStackControllers
{
    using System;
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
        
        public void Hide<T>() where T : Component, IView
        {
            LayoutController.Hide<T>();
        }

        public void HideAll()
        {
            LayoutController.HideAll();
        }

        public void HideAll<T>() where T : Component, IView
        {
            LayoutController.HideAll<T>();
        }

        public void Close<T>() where T : Component, IView
        {
            LayoutController.Close<T>();
        }

        public void Push<TView>(TView view) where TView : Component, IView
        {
            LayoutController.Push(view);
        }

        public bool Close<T>(T view) where T : Component, IView
        {
            return LayoutController.Close<T>(view);
        }

        public TView Get<TView>() where TView : Component, IView
        {
            return LayoutController.Get<TView>();
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