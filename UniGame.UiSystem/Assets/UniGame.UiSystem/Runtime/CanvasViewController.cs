using UnityEngine;

namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    public class CanvasViewController : ViewStackController
    {
        private readonly Canvas canvas;

        #region constructor
        
        public CanvasViewController(Canvas canvas) 
        {
            this.canvas = canvas;
        }
        
        #endregion

        protected override void OnViewAdded<T>(T view)
        {
            view.transform.SetParent(canvas.transform);
        }
    }
}
