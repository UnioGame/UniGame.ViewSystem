namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UnityEngine;

    public interface IViewLayout : 
        IDisposable, 
        IReadOnlyViewLayout
    {

        void Push(IView view);

        void HideAll();

        void CloseAll();

    }
}