namespace UniGame.ViewSystem.Runtime
{
    using R3;


    public interface IViewStatus
    {
        ReadOnlyReactiveProperty<ViewStatus> Status { get; }
    }
}