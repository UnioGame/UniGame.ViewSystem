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

        /// <summary>
        /// is view lifetime finished
        /// </summary>
        bool IsTerminated { get; }

        UniTask Initialize(IViewModel vm);
        UniTask Initialize(IViewModel model, bool disposePrevious);
    }
}