using UniGame.AddressableTools.Runtime;

namespace UniGame.UiSystem.Runtime.Settings
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Core.Runtime;
    using System;
    using System.Collections.Generic;
    using Game.Modules.unigame.unimodules.UniGame.ViewSystem.Runtime.ViewsFactories;
    using Game.Modules.ViewSystem;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.Runtime.DataFlow;
    using UniGame.Attributes;
    using ViewSystem.Runtime;
    using UnityEngine;
    using ViewsFlow;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    /// <summary>
    /// Base View system settings. Contains info about all available view abd type info
    /// </summary>
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/ViewSystemSettings", fileName = nameof(ViewSystemSettings))]
    public class ViewSystemSettings : ViewsSettings, ICompletionStatus, IDisposable
    {
        #region inspector

#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
        [Searchable]
        [GUIColor(1f,0.55f,0.33f)]
#endif
        [Space]
        [Header("Nested Views Sources")]
        [SerializeField] 
        public List<NestedViewSourceSettings> sources = new();

#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
        [DrawWithUnity]
        [InlineEditor]
        [GUIColor(0.8f,0.8f,0.2f)]
#endif
        [SerializeField]
        [AssetFilter(typeof(ViewFlowControllerAsset))]
        public ViewFlowControllerAsset layoutFlow;

        [Space(8)]
        [Header("ViewModels Resolver")]
        [SerializeField]
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
        [InlineProperty]
        [HideLabel]
        [GUIColor(0.5f,0.8f,0.4f)]
#endif
        public ViewModelResolver viewModelResolver = new();

        [Space(8)]
        [Header("Views Factory")]
        [SerializeReference]
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
        [InlineProperty]
        [HideLabel]
        [GUIColor(0.5f,0.8f,0.4f)]
#endif
        public IViewFactoryProvider viewsFactory = new DefaultViewFactoryProvider();
        
        #endregion

        private UiResourceProvider uiResourceProvider;

        private LifeTime _lifeTime = new();

        [NonSerialized] private bool isStarted;

        public bool IsComplete { get; private set; } = false;

        public IViewResourceProvider ResourceProvider => uiResourceProvider;

        public IViewModelTypeMap ViewModelTypeMap => uiResourceProvider;

        public IViewFlowController FlowController { get; protected set; }
        
        public ILifeTime LifeTime => _lifeTime;

        #region public methods


#if UNITY_EDITOR     
        public IEnumerable<string> GetEditorViewsId()
        {
            foreach (var reference in GetEditorViewsId(uiViews))
            {
                yield return reference;
            }

            foreach (var viewSource in sources)
            {
                var settings = viewSource.viewSourceReference.editorAsset;
                foreach (var reference in GetEditorViewsId(settings.uiViews))
                {
                    yield return reference;
                }
            }
            yield break;
        }

        public IEnumerable<string> GetEditorViewsId(IEnumerable<UiViewReference> viewReferences)
        {
            foreach (var uiView in viewReferences)
            {
                var type = uiView.Type.Type;
                var typeName = type.Name;
                var viewName = uiView.ViewName;
                
                if (typeName.Equals(viewName, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return viewName;
                    continue;
                }

                yield return typeName;
                yield return viewName;
            }
        }

#endif
        
        public async UniTask WaitForInitialize()
        {
            while (!IsComplete)
            {
                await UniTask.Yield();
            }
        }

        public void Dispose() => _lifeTime?.Release();

        public async UniTask Initialize()
        {
            if (isStarted) return;

            _lifeTime ??= new LifeTime();
            
            IsComplete = false;
            isStarted = true;

            FlowController = layoutFlow.Create();

            uiResourceProvider ??= new UiResourceProvider();
            uiResourceProvider.RegisterViewReferences(Views);
            viewModelResolver?.Initialize();
            
            await DownloadAllAsyncSources();
        }

        #endregion
        
        #region private methods

        private async UniTask DownloadAllAsyncSources()
        {
            IsComplete = false;

            GameLog.Log($"{nameof(IGameViewSystem)} {name} {nameof(DownloadAllAsyncSources)} STARTED");
            
            foreach (var source in sources.Where(x => !x.awaitLoading))
                LoadAsyncSource(source.viewSourceReference).Forget();

            var syncSettings = sources
                .Where(x => x.awaitLoading)
                .Select(x => LoadAsyncSource(x.viewSourceReference));

            await UniTask.WhenAll(syncSettings);
            
            GameLog.Log($"{nameof(IGameViewSystem)} {name} {nameof(DownloadAllAsyncSources)} COMPLETE");
            
            IsComplete = true;
        }

        private async UniTask LoadAsyncSource(AssetReferenceViewSource reference)
        {
            try
            {
                GameLog.Log($"{nameof(IGameViewSystem)} {name} {nameof(DownloadAllAsyncSources)} STARTED");
                
                var settingsAsset = await reference.LoadAssetTaskAsync(LifeTime);
                if (!settingsAsset)
                {
                    GameLog.LogError($"UiManagerSettings Load EMPTY Settings {reference.AssetGUID}");
                    return;
                }
                
                var settings = Instantiate(settingsAsset);
                settings.DestroyWith(LifeTime);

                uiResourceProvider.RegisterViewReferences(settings.Views);
                
                GameLog.Log($"{nameof(IGameViewSystem)} {name} {nameof(DownloadAllAsyncSources)} STARTED");
            }
            catch (Exception e)
            {
                GameLog.LogError($"UiManagerSettings Load Ui Source failed {reference.AssetGUID}");
                GameLog.LogError(e);
            }
        }

        private void OnDestroy() => Dispose();

        #endregion

        
    }
}