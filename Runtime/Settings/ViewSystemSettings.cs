﻿namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using Addressables.Reactive;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.Core.Runtime.Attributes;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniRx;
    using UnityEngine;
    using ViewsFlow;

    /// <summary>
    /// Base View system settings. Contains info about all available view abd type info
    /// </summary>
    [CreateAssetMenu(menuName = "UniGame/UiSystem/UiSystemSettings", fileName = "UiSystemSettings")]
    public class ViewSystemSettings : ViewsSettings
    {
        [SerializeField]
        private List<UiViewsSourceReference> sources = new List<UiViewsSourceReference>();

        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [Space]
        [Tooltip("Layout Flow Behaviour")]
        [SerializeField]
        [AssetDropDown(typeof(ViewFlowControllerAsset))]
        private ViewFlowControllerAsset layoutFlow;

        private                 LifeTimeDefinition lifeTimeDefinition;
        private                 UiResourceProvider uiResourceProvider;
        [NonSerialized] private bool               isInitialized;

        public void Dispose() => lifeTimeDefinition?.Terminate();

        public IViewResourceProvider<Component> ResourceProvider => uiResourceProvider;

        public IViewFlowController FlowController { get; protected set; }

        public void Initialize()
        {
            if (isInitialized) return;
            
            isInitialized = true;

            FlowController = layoutFlow.Create();
            
            lifeTimeDefinition = lifeTimeDefinition ?? new LifeTimeDefinition();
            uiResourceProvider = uiResourceProvider ?? new UiResourceProvider();

            uiResourceProvider.RegisterViews(uiViews);

            DownloadAllAsyncSources(lifeTimeDefinition.LifeTime);
        }

        #region private methods
        
        private void DownloadAllAsyncSources(ILifeTime lifeTime)
        {
            //load ui views async
            foreach (var reference in sources) {
                reference.
                    ToObservable().
                    Catch<ViewsSettings, Exception>(
                        x => {
                            GameLog.LogError($"UiManagerSettings Load Ui Source failed {reference.AssetGUID}");
                            GameLog.LogError(x);
                            return Observable.Empty<ViewsSettings>();
                        }).
                    Where(x => x != null).
                    Do(x => uiResourceProvider.RegisterViews(x.uiViews)).
                    Subscribe().
                    AddTo(lifeTime);
            }
        }

        private void OnDisable()
        {
            Dispose();
        }

        #endregion
    }
}