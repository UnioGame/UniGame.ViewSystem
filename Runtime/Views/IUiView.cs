namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UnityEngine;

    public interface IUiView<TViewModel> : IView
        where TViewModel : class, IViewModel
    {
        CanvasGroup   CanvasGroup   { get; }
        TViewModel    Model         { get; }
    }
}