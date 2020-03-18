namespace UniGame.UiSystem.Runtime.Abstracts
{
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UnityEngine;

    public interface IReadOnlyViewLayoutContainer
    {
        /// <summary>
        /// indexer of view stack controller
        /// </summary>
        IReadOnlyViewLayout this[ViewType type] { get; }

        TView Get<TView>()  where TView : Component, IView;
    }
}