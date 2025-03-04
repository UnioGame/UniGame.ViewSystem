namespace UniGame.UiSystem.Runtime
{
    using UniGame.AddressableTools.Runtime;
    using UniGame.ViewSystem.Runtime.Extensions;
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Settings;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.UiSystem.Runtime;
    using Core.Runtime;
    using Game.Modules.unigame.unimodules.UniGame.ViewSystem.Runtime.ViewsFactories;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.UISystem.Runtime.WindowStackControllers.Abstract;
    using ViewSystem.Runtime;
    using UnityEngine.AddressableAssets;
    using WindowStackControllers;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    public class GameViewSystemAsset : MonoBehaviour, IGameViewSystem
    {
        #region inspector data
        
#if ODIN_INSPECTOR
        [Required]
        [DrawWithUnity]
#endif
        public AssetReferenceT<ViewSystemSettings> settings;
        
        [Space]
        public ViewLayoutMap layoutMap = new();

        [Space]
        [ListDrawerSettings(ListElementLabelName = "@name")]
        public List<ViewLayoutItem> customLayouts = new();
        
        [Space]
        public ViewLayoutType defaultLayout;
        
        #endregion

        private IGameViewSystem    _gameViewSystem;
        private LifeTimeDefinition _lifeTime = new();

        #region IViewModelProvider api

        public bool IsValid(Type modelType) => ViewSystem.IsValid(modelType);

        public async UniTask<IViewModel> CreateViewModel(IContext context,Type modelType) => await ViewSystem.CreateViewModel(context,modelType);
        
        #endregion  
        
        #region view system api

        public IViewModelTypeMap ModelTypeMap => ViewSystem.ModelTypeMap;
        
        public IObservable<IView> ViewCreated => ViewSystem.ViewCreated;
        
        public IGameViewSystem ViewSystem => _gameViewSystem;

        public bool IsReady => _gameViewSystem != null;

        public ILifeTime LifeTime => _lifeTime;

        public IEnumerable<IViewLayout> Controllers => ViewSystem.Controllers;
        
        public IReadOnlyViewLayout this[ViewType type] => ViewSystem[type];

        public IObservable<TView> ObserveView<TView>() where  TView :class, IView => ViewSystem.ObserveView<TView>();


        public async UniTask<IView> OpenWindow(string viewType, string skinTag = "", string viewName = null)
        {
            return await ViewSystem.OpenWindow(viewType, skinTag, viewName);
        }
        public async UniTask<IView> OpenScreen(string viewType, string skinTag = "", string viewName = null){
            return await ViewSystem.OpenScreen(viewType, skinTag, viewName);
        }
        public async UniTask<IView> OpenOverlay(string viewType, string skinTag = "", string viewName = null){
            return await ViewSystem.OpenOverlay(viewType, skinTag, viewName);
        }

        public async UniTask<IView> Open(string viewType, string layout, string skinTag = "", string viewName = null)
        {
            return await ViewSystem.Open(viewType, layout,skinTag, viewName);
        }

        public async UniTask<IView> CreateWindow(string viewType, string skinTag = "", string viewName = null){
            return await ViewSystem.CreateWindow(viewType, skinTag, viewName);
        }
        public async UniTask<IView> CreateScreen(string viewType, string skinTag = "", string viewName = null){
            return await ViewSystem.CreateScreen(viewType, skinTag, viewName);
        }
        public async UniTask<IView> CreateOverlay(string viewType, string skinTag = "", string viewName = null){
            return await ViewSystem.CreateOverlay(viewType, skinTag, viewName);
        }


        public async UniTask<IView> Create(Type viewType, Transform parent = null,string skinTag = "",string viewName = null,
            bool stayWorldPosition = false, ILifeTime ownerLifeTime = null)
        {
            return await ViewSystem.Create(viewType, parent,skinTag, viewName, stayWorldPosition, ownerLifeTime);
        }

        public UniTask<IView> Create(IViewModel viewModel, string viewType, string skinTag = "", Transform parent = null, string viewName = null, bool stayWorld = false,ILifeTime ownerLifeTime = null) =>
            ViewSystem.Create(viewModel, viewType, skinTag, parent, viewName,stayWorld,ownerLifeTime);

        public UniTask<IView> OpenWindow(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenWindow(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> OpenScreen(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenScreen(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> OpenOverlay(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenOverlay(viewModel, viewType, skinTag, viewName);
        
        public UniTask<IView> CreateWindow(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null) => ViewSystem.CreateWindow(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> CreateScreen(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null) => ViewSystem.CreateScreen(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> CreateOverlay(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null) => ViewSystem.CreateOverlay(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> Create(string viewType, ViewType layoutType, 
            string skinTag = "", string viewName = null, ILifeTime ownerLifeTime = null)
        {
            return ViewSystem.Create(viewType, layoutType, skinTag, viewName, ownerLifeTime);
        }
        
        public UniTask<IView> Create(string viewType, string layoutType, 
            string skinTag = "", string viewName = null, ILifeTime ownerLifeTime = null)
        {
            return ViewSystem.Create(viewType, layoutType, skinTag, viewName, ownerLifeTime);
        }

        public bool HasLayout(string id)
        {
            return ViewSystem.HasLayout(id);
        }

        public bool RegisterLayout(string id, IViewLayout layout)
        {
            return ViewSystem.RegisterLayout(id, layout);
        }

        public bool RemoveLayout(string id)
        {
            return ViewSystem.RemoveLayout(id);
        }
        
        public IViewLayout GetLayout(ViewType type) => ViewSystem.GetLayout(type);
        public IViewLayout GetLayout(string id) => ViewSystem.GetLayout(id);

        public T Get<T>() where T : class, IView
        {
            return ViewSystem.Get<T>();
        }

        public void CloseAll() => ViewSystem.CloseAll();
        
        public void CloseAll(string layoutId)
        {
            ViewSystem.CloseAll(layoutId);
        }

        public void CloseAll(ViewType viewType) => ViewSystem.CloseAll(viewType);
        
        public UniTask<IView> OpenView(IViewModel viewModel, string viewType, string layout, string skinTag = "", string viewName = null)
        {
            return ViewSystem.OpenView(viewModel, viewType, layout, skinTag, viewName);
        }

        public UniTask<T> InitializeView<T>(T view, IViewModel viewModel) where T : IView
        {
            return _gameViewSystem.InitializeView(view, viewModel);
        }

        public void Dispose() => _lifeTime.Terminate();

        #endregion
        
        private void Awake()
        {
            _lifeTime = new LifeTimeDefinition();
            
            Create().AttachExternalCancellation(_lifeTime.Token)
                .Forget();
        }

        private void OnDestroy() => Dispose();

        private async UniTask<IGameViewSystem> Create()
        {
            GameLog.Log($"{nameof(IGameViewSystem)} {name} CreateSystem STARTED {DateTime.Now.ToLongTimeString()}");
            
            var settingsAsset = await settings.LoadAssetTaskAsync(LifeTime);
            settingsAsset = Instantiate(settingsAsset);
            settingsAsset.DestroyWith(LifeTime);
            
            await settingsAsset.Initialize();

            //create view factory specific for settings
            var viewProvider = settingsAsset.viewsFactory ?? new DefaultViewFactoryProvider();
            var factory  = await viewProvider.CreateViewFactoryAsync(settingsAsset);
            
            //fill layouts
            var stackMap = new Dictionary<string, IViewLayout>(4);
            
            foreach (var item in layoutMap)
                stackMap[item.Key.ToStringFromCache()] = item.Value;

            foreach (var customLayout in customLayouts)
                stackMap[customLayout.name] = customLayout.layout;
            
            var viewLayoutContainer = new ViewStackLayoutsContainer(stackMap);
            var sceneFlowController = settingsAsset.FlowController;

            var gameSystem = new GameViewSystem(factory, 
                viewLayoutContainer, 
                sceneFlowController,
                settingsAsset.viewModelResolver,
                settingsAsset.ViewModelTypeMap);
            
            gameSystem.TryMakeActive();

            _gameViewSystem = gameSystem.AddTo(LifeTime);
            
            return gameSystem;
        }

    }

    [Serializable]
    public class ViewLayoutItem
    {
        public string name;
        public ViewSystemLayoutComponent layout;
    }
}
