using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.Extensions;
using UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Abstract;

namespace UniGame.UiSystem.Runtime.Settings
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;
    using System;
    using System.Collections.Generic;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniGame.Core.Runtime.Attributes;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UnityEngine;
    using ViewsFlow;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Base View system settings. Contains info about all available view abd type info
    /// </summary>
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/ViewSystemSettings", fileName = nameof(ViewSystemSettings))]
    public class ViewSystemSettings : ViewsSettings, ICompletionStatus
    {
        #region inspector

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Searchable]
        [Sirenix.OdinInspector.GUIColor(1f,0.55f,0.33f)]
#endif
        [Space]
        [Header("Nested Views Sources")]
        [SerializeField] 
        public List<NestedViewSourceSettings> sources = new List<NestedViewSourceSettings>();

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
        [Sirenix.OdinInspector.InlineEditor]
        [Sirenix.OdinInspector.GUIColor(0.8f,0.8f,0.2f)]
#endif
        [SerializeField]
        [AssetFilter(typeof(ViewFlowControllerAsset))]
        public ViewFlowControllerAsset layoutFlow;

        [Header("ViewModels dependency resolvers")]
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
        [Sirenix.OdinInspector.GUIColor(0.5f,0.8f,0.4f)]
#endif
        public ViewModelResolverSettings viewModelResolvers = new ViewModelResolverSettings();

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
            viewModelResolvers?.Initialize();
            
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
                
                var settings = Object.Instantiate(settingsAsset);
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