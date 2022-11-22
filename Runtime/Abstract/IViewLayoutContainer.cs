namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System.Collections.Generic;
    using UniModules.UniGame.UiSystem.Runtime;

    public interface IViewLayoutContainer : IReadOnlyViewLayoutContainer
    {
        IEnumerable<IViewLayout> Controllers { get; }

        /// <summary>
        /// get controller of target view type 
        /// </summary>
        IViewLayout GetLayout(ViewType type);

        /// <summary>
        /// get layout by string id
        /// </summary>
        IViewLayout GetLayout(string id);
    }
}