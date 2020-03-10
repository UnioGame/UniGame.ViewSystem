namespace UniGreenModules.UniGame.UiSystem.Runtime.Settings
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using AddressableTools.Runtime.Attributes;
    using Sirenix.OdinInspector;
    using Taktika.Addressables.Reactive;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Taktika/Ui/ViewSystemSettings", fileName = "ViewSystemSettings")]
    public class ViewSystemSettings : UiViewsSource, IViewSystemSettings
    {
        [SerializeField]
        [ShowAssetReference]
        [DrawWithUnity]
        private List<UiViewsSourceReference> sources = new List<UiViewsSourceReference>();

        private                 LifeTimeDefinition    lifeTimeDefinition;
        private                 IViewResourceProvider uiResourceProvider;
        [NonSerialized] private bool                  isInitialized;

        public void Dispose() => lifeTimeDefinition?.Terminate();

        public IViewResourceProvider UIResourceProvider => uiResourceProvider;
        
        public void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

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
                    Catch<UiViewsSource, Exception>(
                        x => {
                            GameLog.LogError($"UiManagerSettings Load Ui Source failed {reference.AssetGUID}");
                            GameLog.LogError(x);
                            return Observable.Empty<UiViewsSource>();}).
                    Where(x => x != null).
                    Do(x => uiResourceProvider.RegisterViews(x.uiViews)).Subscribe().AddTo(lifeTime);
            }
        }

        private void OnDisable()
        {
            Dispose();
        }

        #endregion
    }
}