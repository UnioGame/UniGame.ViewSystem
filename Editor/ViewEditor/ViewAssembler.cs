using UniModules.UniCore.Runtime.DataFlow;
using UniGame.Core.Runtime;
using UniModules.UniGame.ViewSystem;
using UniRx;

namespace UniGame.UiSystem.UI.Editor.UiEdito
{
    using Runtime.Settings;
    using UnityEditor;

    public static class ViewAssembler 
    {
        private static ViewsAssemblyBuilder settingsBuilder = new ViewsAssemblyBuilder();
        private static LifeTimeDefinition _lifeTime = new LifeTimeDefinition();
        
        public static ILifeTime LifeTime => (_lifeTime ??= new LifeTimeDefinition());
        
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            _lifeTime ??= new LifeTimeDefinition();
            _lifeTime?.Release();
            
            MessageBroker.Default
                .Receive<SettingsRebuildMessage>()
                .Where(x => x.ViewsSettings!=null)
                .Subscribe(x => x.ViewsSettings.Build())
                .AddTo(LifeTime);
            
            MessageBroker.Default
                .Receive<SettingsRebuildMessage>()
                .Where(x => x.ViewsSettings==null)
                .Subscribe(x => RefreshUiSettings())
                .AddTo(LifeTime);
        }
        
        [MenuItem(itemName:"UniGame/View System/Rebuild View Settings")]
        public static void RefreshUiSettings()
        {
            settingsBuilder.RebuildAll();
        }
        
        [MenuItem(itemName:"UniGame/View System/Update Skin Tags")]
        public static void RefreshSkinTags()
        {
            settingsBuilder.UpdateSkins();
        }
        
        public static void Build(this ViewsSettings settings)
        {
            if (settings == null)
                return;
            settingsBuilder.Build(settings);
        }

        
        [MenuItem(itemName:"Assets/Rebuild ViewsSettings")]
        public static void RebuildSelected()
        {
            Build(Selection.activeObject as ViewsSettings);
        }
    }
}
