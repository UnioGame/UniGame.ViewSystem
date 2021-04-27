namespace UniGame.UiSystem.Runtime.Backgrounds.Abstract
{
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UnityEngine;

    public abstract class BackgroundFactory : MonoBehaviour, IFactory<IBackgroundView>
    {
        public abstract IBackgroundView Create();
    }
}