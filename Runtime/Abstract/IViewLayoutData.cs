namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;

    public interface IViewLayoutData
    {
        IObservable<TView> ObserveView<TView>() where TView :class, IView;

        
        IObservable<IView> ViewCreated { get; }

    }
}