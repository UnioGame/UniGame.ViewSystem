namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using Addressables.Reactive;
    using Sirenix.OdinInspector;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Attributes;
    using UniRx;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/UiSystem/ViewSystemSettings", fileName = "ViewSystemSettings")]
    public class ViewSystemSettings : ViewsSource, IViewSystemSettings
    {
        [SerializeField]
        [ShowAssetReference]
        [DrawWithUnity]
        private List<UiViewsSourceReference> sources = new List<UiViewsSourceReference>();

        private                 LifeTimeDefinition lifeTimeDefinition;
        private                 UiResourceProvider uiResourceProvider;
        [NonSerialized] private bool               isInitialized;

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
        // TO DO
        // Непонятно загрузились ли нужные ресурсы, появляются неявные связи 
        // например в лобби нужно в сырсах указывать ещё и сырсы UI
        // Возможности загрузить через ContextNode нету
        private void DownloadAllAsyncSources(ILifeTime lifeTime)
        {
            //load ui views async
            foreach (var reference in sources) {
                reference.ToObservable().Catch<ViewsSource, Exception>(
                    x => {
                        GameLog.LogError($"UiManagerSettings Load Ui Source failed {reference.AssetGUID}");
                        GameLog.LogError(x);
                        return Observable.Empty<ViewsSource>();
                    }).
                    Where(x => x != null).
                    Do(
                        x => uiResourceProvider.RegisterViews(x.uiViews)).
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