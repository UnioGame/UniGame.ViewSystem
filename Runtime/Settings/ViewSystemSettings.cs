using System.Linq;
using Cysharp.Threading.Tasks;
using UniModules.UniGame.Core.Runtime.Interfaces;

namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using System.Collections.Generic;
    using Addressables.Reactive;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.Core.Runtime.Attributes;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    using UnityEngine;
    using ViewsFlow;

    /// <summary>
    /// Base View system settings. Contains info about all available view abd type info
    /// </summary>
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/ViewSystemSettings", fileName = nameof(ViewSystemSettings))]
    public class ViewSystemSettings : ViewsSettings, ICompletionStatus
    {
        [SerializeField]
        private List<NestedViewSourceSettings> sources = new List<NestedViewSourceSettings>();

        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [Space]
        [Tooltip("Layout Flow Behaviour")]
        [SerializeField]
        [AssetFilter(typeof(ViewFlowControllerAsset))]
        private ViewFlowControllerAsset layoutFlow;

        private                 LifeTimeDefinition lifeTimeDefinition;
        private                 UiResourceProvider uiResourceProvider;
        [NonSerialized] private bool               isStarted;

        public void Dispose() => lifeTimeDefinition?.Terminate();
        
        public bool IsComplete { get; private set; } = false;
        
        public IViewResourceProvider<Component> ResourceProvider => uiResourceProvider;

        public IViewFlowController FlowController { get; protected set; }

        public async UniTask WaitForInitialize()
        {
            while (!LifeTime.IsTerminated && !IsComplete)
            {
                await UniTask.Yield();
            }
        }
        
        public void Initialize()
        {
            if (isStarted) return;

            IsComplete = false;
            isStarted = true;

            FlowController = layoutFlow.Create();
            
            lifeTimeDefinition = lifeTimeDefinition ?? new LifeTimeDefinition();
            uiResourceProvider = uiResourceProvider ?? new UiResourceProvider();

            uiResourceProvider.RegisterViews(uiViews);

            DownloadAllAsyncSources();  
        }

        #region private methods
        
        private async UniTask DownloadAllAsyncSources()
        {
            IsComplete = false;

            foreach (var source in sources.Where(x => !x.awaitLoading))
            {
                LoadAsyncSource(source.viewSourceReference);
            }
            
            //load ui views async
            foreach (var viewSource in sources.Where(x => x.awaitLoading))
            {
                await LoadAsyncSource(viewSource.viewSourceReference);
            }

            IsComplete = true;
        }

        private async UniTask LoadAsyncSource(AssetReferenceViewSource reference)
        {
            try
            {
                var settings = await reference
                    .ToObservable()
                    .First();
                
                if (!settings)
                {
                    GameLog.LogError($"UiManagerSettings Load EMPTY Settings {reference.AssetGUID}");
                    return;
                }
                uiResourceProvider.RegisterViews(settings.uiViews);
            }
            catch (Exception e)
            {
                GameLog.LogError($"UiManagerSettings Load Ui Source failed {reference.AssetGUID}");
                GameLog.LogError(e);
            }
            
        }

        private void OnDisable()
        {
            Dispose();
        }

        #endregion

    }
}