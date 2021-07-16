using UnityEngine;

namespace UniGame.UiSystem.Runtime.Backgrounds.Abstract
{
    public interface IBackgroundFactory
    {
        IBackgroundView Create(Transform parent);
    }
}