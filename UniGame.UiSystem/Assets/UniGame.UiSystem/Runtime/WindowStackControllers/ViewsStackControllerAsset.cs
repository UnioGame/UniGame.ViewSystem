using UnityEngine;

namespace UniGame.UiSystem.Runtime.WindowStackControllers
{
    using System;
    using Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;

    public class ViewsStackControllerAsset : MonoBehaviour, IViewLayoutController
    {
        #region inspector

        public Canvas layoutCanvas;

        #endregion

        private Lazy<IViewLayoutController> stackController;

        public IViewLayoutController StackController => stackController.Value;

        public Transform Layout => StackController.Layout;

        #region public methods

        public void Dispose() => StackController.Dispose();

        public bool Contains(IView view) => StackController.Contains(view);

        public void Add<TView>(TView view) where TView : Component, IView
        {
            StackController.Add(view);
        }

        public bool Remove<T>(T view) where T : Component, IView
        {
            return StackController.Remove(view);
        }

        public void Hide<T>() where T : Component, IView
        {
            StackController.Hide<T>();
        }

        public void HideAll()
        {
            StackController.HideAll();
        }

        public void HideAll<T>() where T : Component, IView
        {
            StackController.HideAll<T>();
        }

        public void Close<T>() where T : Component, IView
        {
            StackController.Close<T>();
        }

        public void CloseAll()
        {
            StackController.CloseAll();
        }

        #endregion


        #region private methods

        protected virtual IViewLayoutController Create()
        {
            return new CanvasViewController(layoutCanvas);
        }

        protected void Awake()
        {
            stackController = new Lazy<IViewLayoutController>(Create);
        }

        #endregion
    }
}