namespace UniGame.UiSystem.Runtime.Backgrounds.Abstract
{
    using UnityEngine;

    public abstract class BackgroundFactory : MonoBehaviour,IBackgroundFactory
    {
        public abstract IBackgroundView Create(Transform parent);
    }
}