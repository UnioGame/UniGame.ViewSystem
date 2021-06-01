using UniRx;

namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;

    public interface IViewLayout : 
        IDisposable,
        IReadOnlyViewLayout
    {
        IReadOnlyReactiveProperty<bool> HasActiveView { get; }

        void Push(IView view);

        void HideAll();

        void CloseAll();

        void ShowLast();

        void Suspend();
        void Resume();
    }
}