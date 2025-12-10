namespace UniGame.ViewSystem.Runtime
{
    using System;
    using UniModules.UniGame.UiSystem.Runtime;

    public interface IReadOnlyViewLayoutContainer
    {
        /// <summary>
        /// indexer of view stack controller
        /// </summary>
        IReadOnlyViewLayout this[ViewType type] { get; }

        TView GetView<TView>()  where TView : class,IView;

        IView GetView(Type viewType);
    }
}