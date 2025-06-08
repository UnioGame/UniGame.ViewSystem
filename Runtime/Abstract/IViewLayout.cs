 

namespace UniGame.ViewSystem.Runtime
{
    using System;
    using Core.Runtime;
    using R3;
    using UiSystem.Runtime;

    public interface IViewLayout : 
        IDisposable,
        ILifeTimeContext,
        IReadOnlyViewLayout
    {
        ReadOnlyReactiveProperty<IView> ActiveView { get; }

        LayoutIntentResult Intent(string viewKey);
        
        void Push(IView view);

        void HideAll();

        void CloseAll();

        void ShowLast();

        void Suspend();
        
        void Resume();
    }
}