using UniModules.UniGame.UISystem.Runtime;
using UnityEngine;

namespace UniGame.ViewSystem.Runtime
{
    using System;
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UniRx;


    public interface IView : 
        ILifeTimeContext, 
        IViewStatus, 
        IViewCommands
    {
        public GameObject Owner { get; }
        
        public GameObject GameObject { get; }

        public Transform  Transform { get; }

        ILifeTime ViewLifeTime { get; }
        
        string ViewId { get; }
        
        int ViewIdHash { get; }
        
        IReadOnlyReactiveProperty<bool> IsVisible { get; }

        IReadOnlyReactiveProperty<bool> IsInitialized { get; }

        //IViewModel ViewModel { get; }
        
        IObservable<IView> SelectStatus(ViewStatus status);

        IViewModel ViewModel { get; }
        
        string SourceName { get; }
        
        /// <summary>
        /// is view lifetime finished
        /// </summary>
        bool IsTerminated { get; }
        
        bool IsModelAttached { get; }
        
        /// <summary>
        /// setup name of source asset
        /// </summary>
        /// <param name="sourceName"></param>
        void SetSourceName(string viewId,string sourceName);

        UniTask<IView> Initialize(IViewModel vm, bool ownViewModel = false);
    }
    
    public interface IModelView : IView
    {
        ILifeTime ModelLifeTime { get; }
        
    }
}