namespace Taktika.UI.Common.MagneticViews.Abstract
{
    using UnityEngine;

    public interface IFollowingUI
    {
        void Init(RectTransform bounds, IFollowable target);
    }
}