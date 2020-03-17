using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;

    public class CanvasViewLayout : ViewLayout
    {
        private readonly Canvas canvas;

        #region constructor
        
        public CanvasViewLayout(Canvas canvas) 
        {
            this.canvas = canvas;
            Layout = canvas?.transform;
        }

        #endregion

        public Canvas Canvas => canvas;


        protected override void OnViewAdded<T>(T view)
        {
            var viewLifetime = view.LifeTime;
            view.IsActive.
                Where(x => x).
                Subscribe(x => view.transform.SetAsLastSibling()).
                AddTo(viewLifetime);
            
            if (view.transform.parent == Layout)
                return;

            view.transform.SetParent(Layout);
        }
        
    }
}
