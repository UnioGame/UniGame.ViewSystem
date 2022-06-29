namespace Taktika.UI.Common.MagneticViews.Abstract
{
    using Taktika.UI.Views;
    using UnityEngine;

    public abstract class BaseFollowingUI<T> : UiAnimatorView<T>, IFollowingUI where T : class, IFollowingViewModel
    {
        protected IFollowable   Target;
        protected RectTransform Bounds;

        private void LateUpdate()
        {
            SetPosition();
        }

        public virtual void Init(RectTransform bounds, IFollowable target)
        {
            Target                  = target;
            Bounds                  = bounds;
            RectTransform.anchorMin = Vector2.zero;
            RectTransform.anchorMax = Vector2.zero;
        }
        
        protected virtual void SetPosition()
        {
            if (Target == null)
                return;
            
            var canvasPos = GetBarkBaseOnCanvas(false);
            RectTransform.anchoredPosition = canvasPos;
        }
        
        protected virtual Vector3 GetBarkBaseOnCanvas(bool clampToScreen)
        {
            var screenPos = Target.GetFollowPosition();

            if (clampToScreen)
                screenPos = new Vector3(Mathf.Clamp(screenPos.x, 0, Screen.width), Mathf.Clamp(screenPos.y, 0, Screen.height), screenPos.z);
            
            return screenPos * (1f / Bounds.localScale.x);
        }
    }
}