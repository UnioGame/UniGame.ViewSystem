using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using ViewSystem.Runtime;

    public interface IUiView<TViewModel> : IView
     
        where TViewModel : class, IViewModel
    {
        
        CanvasGroup CanvasGroup { get; }
        Canvas Canvas { get; }
        TViewModel    Model         { get; }
        
    }
}