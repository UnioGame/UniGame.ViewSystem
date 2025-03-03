namespace Game.Modules.unigame.unimodules.UniGame.ViewSystem.Runtime.ViewsFactories
{
    using System;
    using Cysharp.Threading.Tasks;
    using global::UniGame.UiSystem.Runtime;
    using global::UniGame.UiSystem.Runtime.Settings;
    using global::UniGame.ViewSystem.Runtime;
    using Modules.ViewSystem;

    [Serializable]
    public class DefaultViewFactoryProvider : IViewFactoryProvider
    {
        public async UniTask<IViewFactory> CreateViewFactoryAsync(ViewSystemSettings settings)
        {
            var factory  = new ViewFactory(new AsyncLazy(settings.WaitForInitialize),
                settings.ResourceProvider);
            return factory;
        }
    }
}