namespace UniGreenModules.UniGame.UiSystem.Runtime.Abstracts
{
    using System.Collections.Generic;
    using Settings;
    using Taktika.Addressables.Reactive;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniRx.Async;
    using UnityEngine;

    public interface IViewResourceProvider
    {
        /// <summary>
        /// Load view with target type
        /// </summary>
        IAddressableObservable<TView> LoadViewAsync<TView>(
            bool strongMatching = true) 
            where TView : Object;
        
        /// <summary>
        /// Load view by type and target skin tag
        /// </summary>
        /// <returns>found view or null</returns>
        IAddressableObservable<TView> LoadViewAsync<TView>(
            string skinTag,
            bool strongMatching = true)
            where TView : Object;

        /// <summary>
        /// load all Views with target Type
        /// </summary>
        List<IAddressableObservable<TView>> LoadViewsAsync<TView>(
            bool strongMatching = true) 
            where TView : Object;
        
        /// <summary>
        /// load all Views with targte Type and Tag
        /// </summary>
        List<IAddressableObservable<TView>> LoadViewsAsync<TView>(
            string skinTag,
            bool strongMatching = true) 
            where TView : Object;

        void RegisterViews(IReadOnlyList<UiViewDescription> items);
    }
}
