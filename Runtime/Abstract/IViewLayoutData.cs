namespace UniGame.ViewSystem.Runtime
{
    using R3;

    public interface IViewLayoutData
    {
        Observable<TView> ObserveView<TView>() where TView :class, IView;

        
        Observable<IView> ViewCreated { get; }

    }
}