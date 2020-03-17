namespace UniGame.UiSystem.Runtime.Abstracts
{
    using UnityEngine;

    public interface IReadOnlyViewLayout 
    {

        bool Contains(IView view);
        
        void Hide<T>() where T :Component, IView;
        
        void HideAll();
        
        void HideAll<T>() where T : Component, IView;

        void Close<T>() where T :Component, IView;

        TView Get<TView>() where TView : Component, IView;
        
        void CloseAll();

    }
}