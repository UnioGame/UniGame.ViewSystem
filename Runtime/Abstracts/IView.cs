namespace UniGame.UiSystem.Runtime.Abstracts
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;
    
    using UnityEngine;

    public interface IView : 
        ILifeTimeContext, 
        IViewStatus, 
        IViewCommands
    {
        /// <summary>
        /// view owner
        /// </summary>
        GameObject Owner { get; }

        IReadOnlyReactiveProperty<bool> IsVisible { get; }

        /// <summary>
        /// view rect transform
        /// </summary>
        RectTransform RectTransform { get; }

        /// <summary>
        /// view transform
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// is view lifetime finished
        /// </summary>
        bool IsTerminated { get; }

        UniTask Initialize(IViewModel vm);

        /// <summary>
        /// Add view as child
        /// </summary>
        /// <param name="view"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        IView AddView(IView view, bool worldPositionStays = false);

        /// <summary>
        /// create child view
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="skinTag"></param>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        UniTask<IView> CreateView<TView>(IViewModel viewModel, string skinTag = "")
            where TView : class, IView;
    }
}