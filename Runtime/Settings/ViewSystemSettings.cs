using UniGame.AddressableTools.Runtime;
using UniGame.ViewSystem.Runtime.Abstract;

namespace UniGame.UiSystem.Runtime.Settings
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Core.Runtime;
    using System;
    using System.Collections.Generic;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniGame.Core.Runtime.Attributes;
    using ViewSystem.Runtime;
    using UnityEngine;
    using ViewsFlow;
    using Object = UnityEngine.Object;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    /// <summary>
    /// Base View system settings. Contains info about all available view abd type info
    /// </summary>
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/ViewSystemSettings", fileName = nameof(ViewSystemSettings))]
    public class ViewSystemSettings : ViewsSettings, ICompletionStatus
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
        public List<NestedViewSourceSettings> sources = new List<NestedViewSourceSettings>();

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
        public ViewModelResolver viewModelResolver = new ViewModelResolver();

        #endregion

        private UiResourceProvider uiResourceProvider;

        [NonSerialized] private bool isStarted;

        public bool IsComplete { get; private set; } = false;

        public IViewResourceProvider ResourceProvider => uiResourceProvider;

        public IViewModelTypeMap ViewModelTypeMap => uiResourceProvider;

        public IViewFlowController FlowController { get; protected set; }

        #region public methods
        
        public async UniTask WaitForInitialize()
        {
            while (!LifeTime.IsTerminated && !IsComplete)
            {
                await UniTask.Yield();
            }
        }

        public async UniTask Initialize()
        {
            if (isStarted) return;

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

        private void OnDisable() => Dispose();

        #endregion
    }
}