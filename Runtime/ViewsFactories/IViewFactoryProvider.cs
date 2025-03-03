namespace Game.Modules.ViewSystem
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.UiSystem.Runtime.Settings;
    using global::UniGame.ViewSystem.Runtime;

    public interface IViewFactoryProvider
    {
        public UniTask<IViewFactory> CreateViewFactoryAsync(ViewSystemSettings settings);
    }
}