namespace Taktika.UI.Common.Views.Abstract
{
    using UnityEngine;

    public interface IUIPositionHolder
    {
        Vector2 Position { get; }

        void HoldPosition(Vector2 position);
    }
}