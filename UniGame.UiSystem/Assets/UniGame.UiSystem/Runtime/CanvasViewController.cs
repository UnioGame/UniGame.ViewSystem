using UnityEngine;

namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using Abstracts;

    public class CanvasViewController : ViewController
    {
        private readonly Canvas canvas;

        #region constructor
        
        public CanvasViewController(
            Canvas canvas,
            IViewFactory viewFactory,
            IViewElementFactory elementFactory) : base(viewFactory,elementFactory)
        {
            this.canvas = canvas;
        }
        
        #endregion

        protected override void OnViewOpen<T>(T view)
        {
            view.transform.SetParent(canvas.transform);
        }
    }
}
