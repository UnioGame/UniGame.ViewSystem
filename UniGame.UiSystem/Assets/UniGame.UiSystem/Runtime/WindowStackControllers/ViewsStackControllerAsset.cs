using UnityEngine;

namespace UniGame.UiSystem.Runtime.WindowStackControllers
{
    using System;
    using Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;

    public class ViewsStackControllerAsset : MonoBehaviour, IViewStackController
    {
        #region inspector

        public Canvas layoutCanvas;

        #endregion

        private Lazy<IViewStackController> stackController;

        public IViewStackController StackController => stackController.Value;

        public IObservable<IView> StackTopChanged => stackController.Value.StackTopChanged;
        
        public Transform Layout => StackController.Layout;

        #region public methods

        public ViewsStackControllerAsset()
        {
            stackController = new Lazy<IViewStackController>(Create);
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

        protected virtual IViewStackController Create()
        {
            return new ScreenViewStackController(layoutCanvas.transform);
        }
        
        #endregion
    }
}