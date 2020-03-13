namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System.Collections.Generic;
    using UniGreenModules.UniGame.UiSystem.Runtime;

    public interface IViewLayoutContainer : IReadOnlyViewLayoutContainer
    {
        IEnumerable<IViewLayoutController> Controllers { get; }

        /// <summary>
        /// get controller of target view type 
        /// </summary>
        IViewLayoutController GetViewController(ViewType type);
    }
}