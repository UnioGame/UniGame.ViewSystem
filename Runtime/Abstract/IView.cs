namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniRx;

    public interface IView : 
        ILifeTimeContext, 
        IViewStatus, 
        IViewCommands
    {
        IReadOnlyReactiveProperty<bool> IsVisible { get; }

        IReadOnlyReactiveProperty<bool> IsInitialized { get; }

        /// <summary>
        /// is view lifetime finished
        /// </summary>
        bool IsTerminated { get; }

        UniTask Initialize(IViewModel vm, bool isViewOwner = false);
    }
}