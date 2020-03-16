using UnityEngine;

namespace UniGame.UiSystem.Runtime.WindowStackControllers
{
    using System;
    using Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;

    public class ViewsLayoutAsset : MonoBehaviour, IViewLayout
    {
        #region inspector

        public Canvas layoutCanvas;

        #endregion

        private Lazy<IViewLayout> layout;

        public IViewLayout StackController => layout.Value;

        public IObservable<IView> LayoutChanged => layout.Value.LayoutChanged;
        
        public Transform Layout => StackController.Layout;

        #region public methods

        public ViewsLayoutAsset()
        {
            layout = new Lazy<IViewLayout>(Create);
        }

        public void Dispose() => StackController.Dispose();

        public bool Contains(IView view) => StackController.Contains(view);

        public void Add<TView>(TView view) where TView : Component, IView
        {
            StackController.Add(view);
        }

        public TView Get<TView>() where TView : Component, IView
        {
            return StackController.Get<TView>();
        }

        public void CloseAll()
        {
            StackController.CloseAll();
        }

        //public bool Remove<T>(T view) where T : Component, IView
        //{
        //    return StackController.Remove(view);
        //}

        //public void Close<T>() where T : Component, IView
        //{
        //    StackController.Close<T>();
        //}
        
        #endregion


        #region private methods

        protected virtual IViewLayout Create()
        {
            return new ScreenViewStackController(layoutCanvas.transform);
        }
        
        #endregion
    }
}