namespace UniGame.ViewSystem.Runtime
{
    using System.Collections.Generic;
    using UniModules.UniGame.UiSystem.Runtime;

    public interface IViewLayoutContainer : IReadOnlyViewLayoutContainer
    {
        IEnumerable<IViewLayout> Layouts { get; }

        bool HasLayout(string id);
        
        bool RegisterLayout(string id,IViewLayout layout);
        
        bool RemoveLayout(string id);
        
        /// <summary>
        /// get controller of target view type 
        /// </summary>
        IViewLayout GetLayout(ViewType type);

        /// <summary>
        /// get layout by string id
        /// </summary>
        IViewLayout GetLayout(string id);
        
        void CloseAll();

        void CloseAll(string id);
    }
}