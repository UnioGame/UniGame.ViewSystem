using UniRx;

namespace UniGame.ViewSystem.Runtime
{
    using System;
    using UiSystem.Runtime;

    public interface IViewLayout : 
        IDisposable,
        IReadOnlyViewLayout
    {
        IReadOnlyReactiveProperty<IView> ActiveView { get; }

        LayoutIntentResult Intent(string viewKey);
        
        void Push(IView view);

        void HideAll();

        void CloseAll();

        void ShowLast();

        void Suspend();
        
        void Resume();
    }
}