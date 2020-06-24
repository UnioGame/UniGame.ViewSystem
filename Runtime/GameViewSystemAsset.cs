﻿using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using Settings;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniRx.Async;

    public class GameViewSystemAsset : MonoBehaviour, IGameViewSystem
    {
        
        #region inspector data
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Required]
        [Sirenix.OdinInspector.InlineEditor]
#endif
        public ViewSystemSettings settings;
        
        [Space]
        public ViewLayoutMap layoutMap = new ViewLayoutMap();

        #endregion

        private IGameViewSystem gameViewSystem;

        #region view system api

        public IGameViewSystem ViewSystem => gameViewSystem ?? (gameViewSystem = Create());

        public ILifeTime LifeTime => ViewSystem.LifeTime;

        public IEnumerable<IViewLayout> Controllers => ViewSystem.Controllers;
        
        public IReadOnlyViewLayout this[ViewType type] => ViewSystem[type];

        public UniTask<IView> Create(IViewModel viewModel, Type viewType, string skinTag = "", Transform parent = null, string viewName = null) => ViewSystem.Create(viewModel, viewType, skinTag, parent, viewName);

        public UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenWindow(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> OpenScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenScreen(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> OpenOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenOverlay(viewModel, viewType, skinTag, viewName);

        public IViewLayout GetLayout(ViewType type) => ViewSystem.GetLayout(type);

        public T Get<T>() where T : Component, IView
        {
            return ViewSystem.Get<T>();
        }

        public void CloseAll()
        {
            ViewSystem.CloseAll();
        }

        public void Dispose() => gameViewSystem?.Dispose();
        
        #endregion

        private void OnDestroy() => Dispose();

        private IGameViewSystem Create()
        {
            settings.Initialize();
            
            var factory  = new ViewFactory(settings.ResourceProvider);
            var stackMap = new Dictionary<ViewType, IViewLayout>(4);
            foreach (var item in layoutMap) {
                stackMap[item.Key] = item.Value;
            }
            
            var viewLayoutContainer = new ViewStackLayoutsContainer(stackMap);
            var sceneFlowController = settings.FlowController;

            return new GameViewSystem(factory, viewLayoutContainer, sceneFlowController);
        }
    }
}