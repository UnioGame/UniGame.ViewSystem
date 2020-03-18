namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System.Collections.Generic;
    using Addressables.Reactive;
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
        
    }
}
