namespace UniGame.UiSystem.Runtime.Abstracts
{
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UnityEngine;

    public interface IReadOnlyViewLayoutController 
    { 

        bool Contains(IView view);
        
        void Hide<T>() where T :Component, IView;
        void HideAll();
        void HideAll<T>() where T : Component, IView;

        
        void Close<T>() where T :Component, IView;
        void CloseAll();


    }
}