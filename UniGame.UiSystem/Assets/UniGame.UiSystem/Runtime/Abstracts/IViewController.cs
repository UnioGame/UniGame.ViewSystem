namespace UniGreenModules.UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniRx.Async;
    using UnityEngine;
    using Object = UnityEngine.Object;

    // Из названия не очевидно назначение может стоит использовать что то вроде ViewStackController
    public interface IViewController : IDisposable
    {
        UniTask<T> Open<T>(IViewModel viewModel,string skinTag = "") where T :Component, IView;
        
        bool Hide<T>() where T :Component, IView;

        void HideAll();

        void HideAll<T>() where T : Component, IView;
        
        bool Close<T>() where T :Component, IView;
        
        void CloseAll();

        bool Close<T>(T view) where T : Component, IView;

    }
}