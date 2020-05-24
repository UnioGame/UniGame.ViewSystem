namespace UniGame.UiSystem.Runtime.Backgrounds.Abstract
{
    using Abstracts;
    using UnityEngine;

    public interface IBackgroundViewModel : IViewModel
    {
        Material BlurMaterial { get; }
    }
}