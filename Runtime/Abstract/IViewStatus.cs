namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using Runtime;
    using UniRx;

    public interface IViewStatus
    {
        IReadOnlyReactiveProperty<ViewStatus> Status { get; }
    }
}