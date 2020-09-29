namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;

    public interface IViewLayout : 
        IDisposable, 
        IReadOnlyViewLayout
    {
        void Push(IView view);

        void HideAll();

        void CloseAll();

        void ShowLast();
    }
}