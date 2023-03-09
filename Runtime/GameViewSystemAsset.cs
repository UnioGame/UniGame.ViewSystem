namespace UniGame.UiSystem.Runtime
{
    using UniGame.AddressableTools.Runtime;
    using UniGame.ViewSystem.Runtime.Abstract;
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
    using ViewSystem.Runtime;
    using UnityEngine.AddressableAssets;
    using Object = UnityEngine.Object;
    
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
        public ViewLayoutMap layoutMap = new ViewLayoutMap();

        #endregion

        private IGameViewSystem    _gameViewSystem;
        private LifeTimeDefinition _lifeTime;

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


        public async UniTask<IView> OpenWindow(Type viewType, string skinTag = "", string viewName = null)
        {
            return await ViewSystem.OpenWindow(viewType, skinTag, viewName);
        }
        public async UniTask<IView> OpenScreen(Type viewType, string skinTag = "", string viewName = null){
            return await ViewSystem.OpenScreen(viewType, skinTag, viewName);
        }
        public async UniTask<IView> OpenOverlay(Type viewType, string skinTag = "", string viewName = null){
            return await ViewSystem.OpenOverlay(viewType, skinTag, viewName);
        }

        public async UniTask<IView> CreateWindow(Type viewType, string skinTag = "", string viewName = null){
            return await ViewSystem.CreateWindow(viewType, skinTag, viewName);
        }
        public async UniTask<IView> CreateScreen(Type viewType, string skinTag = "", string viewName = null){
            return await ViewSystem.CreateScreen(viewType, skinTag, viewName);
        }
        public async UniTask<IView> CreateOverlay(Type viewType, string skinTag = "", string viewName = null){
            return await ViewSystem.CreateOverlay(viewType, skinTag, viewName);
        }


        public async UniTask<IView> Create(Type viewType, string skinTag = "", Transform parent = null, string viewName = null,
            bool stayWorldPosition = false, ILifeTime ownerLifeTime = null)
        {
            return await ViewSystem.Create(viewType, skinTag, parent, viewName, stayWorldPosition, ownerLifeTime);
        }

        public UniTask<IView> Create(IViewModel viewModel, Type viewType, string skinTag = "", Transform parent = null, string viewName = null, bool stayWorld = false,ILifeTime ownerLifeTime = null) =>
            ViewSystem.Create(viewModel, viewType, skinTag, parent, viewName,stayWorld,ownerLifeTime);

        public UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenWindow(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> OpenScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenScreen(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> OpenOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenOverlay(viewModel, viewType, skinTag, viewName);
        
        public UniTask<IView> CreateWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.CreateWindow(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> CreateScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.CreateScreen(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> CreateOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.CreateOverlay(viewModel, viewType, skinTag, viewName);

        
        public IViewLayout GetLayout(ViewType type) => ViewSystem.GetLayout(type);
        public IViewLayout GetLayout(string id) => ViewSystem.GetLayout(id);

        public T Get<T>() where T : class, IView
        {
            return ViewSystem.Get<T>();
        }

        public void CloseAll() => ViewSystem.CloseAll();
        
        public UniTask<T> InitializeView<T>(T view, IViewModel viewModel) where T : IView
        {
            return _gameViewSystem.InitializeView(view, viewModel);
        }

        public void Dispose() => _lifeTime.Terminate();

        #endregion
        
        private void Awake()
        {
            _lifeTime = new LifeTimeDefinition();
            
            Create().AttachExternalCancellation(_lifeTime.TokenSource)
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

            var factory  = new ViewFactory(new AsyncLazy(settingsAsset.WaitForInitialize),settingsAsset.ResourceProvider);
            var stackMap = new Dictionary<ViewType, IViewLayout>(4);
            
            foreach (var item in layoutMap)
                stackMap[item.Key] = item.Value;
            
            var viewLayoutContainer = new ViewStackLayoutsContainer(stackMap);
            var sceneFlowController = settingsAsset.FlowController;

            var gameSystem = new GameViewSystem(factory, viewLayoutContainer, 
                sceneFlowController,
                settingsAsset.viewModelResolver,
                settingsAsset.ViewModelTypeMap);
            
            gameSystem.TryMakeActive();

            _gameViewSystem = gameSystem.AddTo(LifeTime);
            
            return gameSystem;
        }

    }
}
