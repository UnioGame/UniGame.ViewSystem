using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    public class CanvasViewController : ViewStackController
    {
        private readonly Canvas canvas;

        #region constructor
        
        public CanvasViewController(Canvas canvas) 
        {
            this.canvas = canvas;
            Layout = canvas?.transform;
        }

        #endregion

        public Canvas Canvas => canvas;


        protected override void OnViewAdded<T>(T view)
        {
            if (view.transform.parent == Layout)
                return;

            view.transform.SetParent(Layout);
        }
    }
}
