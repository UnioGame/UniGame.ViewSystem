namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;

    public interface IViewLayout : 
        IDisposable, 
        IReadOnlyViewLayout
    {
        void Push<TView>(TView view) where TView :class, IView;

        void HideAll();

        void CloseAll();
    }
}