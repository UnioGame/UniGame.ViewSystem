namespace Game.Modules.ViewSystem.ZenjectViewModule
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.UiSystem.Runtime.Settings;
    using UniGame.ViewSystem.Runtime;
    using Zenject;

    [Serializable]
    public class ZenjectViewFactoryProvider : IViewFactoryProvider
    {
        public static DiContainer Container { get; set; }
        
        public async UniTask<IViewFactory> CreateViewFactoryAsync(ViewSystemSettings settings)
        {
            await UniTask.WaitWhile(() => Container == null);
            
            var zenjectViewFactory = new ZenjectViewFactory(
                Container,
                new AsyncLazy(settings.WaitForInitialize),
                settings.ResourceProvider);

            return zenjectViewFactory;
        }
    }
}