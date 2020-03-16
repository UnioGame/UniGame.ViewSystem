namespace UniGame.UiSystem.Runtime.Abstracts
{
    using UniGreenModules.UniGame.UiSystem.Runtime;

    public interface IReadOnlyViewLayoutContainer
    {
        /// <summary>
        /// indexer of view stack controller
        /// </summary>
        IReadOnlyViewLayout this[ViewType type] { get; }
    }
}