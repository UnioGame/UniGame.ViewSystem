namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using UniGreenModules.UniGame.UiSystem.Runtime;

    public interface IReadOnlyViewLayoutContainer
    {
        /// <summary>
        /// indexer of view stack controller
        /// </summary>
        IReadOnlyViewLayout this[ViewType type] { get; }

        TView Get<TView>()  where TView : class,IView;
    }
}