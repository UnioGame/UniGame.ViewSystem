namespace UniGame.UiSystem.Runtime
{
    using System;
    using Core.Runtime;
    using R3;
    using UniGame.Runtime.DataFlow;
    using UnityEngine.SceneManagement;
    using ViewSystem.Runtime;

    [Serializable]
    public class BaseFlowController : IViewFlowController
    {
        private LifeTime _lifeTimeDefinition = new();
        
        public IViewLayoutContainer controllerContainer;
        
        public ILifeTime LifeTime => _lifeTimeDefinition;
        
        public void Activate(IViewLayoutContainer controllerContainer)
        {
            _lifeTimeDefinition.Restart();
            _lifeTimeDefinition.AddCleanUpAction(() => this.controllerContainer = null);

            this.controllerContainer = controllerContainer;
            
            BindSceneActions();
            
            OnActivate(controllerContainer);
        }

        public void Dispose() => _lifeTimeDefinition.Terminate();
        
        #region peivate methods
        
        protected virtual void OnSceneActivate(Scene current, Scene next) { }

        protected virtual void OnSceneLoaded(Scene current, LoadSceneMode mode) { }

        protected virtual void OnSceneUnloaded(Scene current) {}

        /// <summary>
        /// Custom defined initialization of layout
        /// </summary>
        /// <param name="layouts"></param>
        protected virtual void OnActivate(IViewLayoutContainer layouts)
        {
        }

        private void BindSceneActions()
        {
            Observable.FromEvent(
                    x => SceneManager.activeSceneChanged += OnSceneActivate,
                    x => SceneManager.activeSceneChanged -= OnSceneActivate).
                Subscribe().
                AddTo(LifeTime);

            Observable.FromEvent(
                    x => SceneManager.sceneLoaded += OnSceneLoaded,
                    x => SceneManager.sceneLoaded -= OnSceneLoaded).
                Subscribe().
                AddTo(LifeTime);

            Observable.FromEvent(
                    x => SceneManager.sceneUnloaded += OnSceneUnloaded,
                    x => SceneManager.sceneUnloaded -= OnSceneUnloaded).
                Subscribe().
                AddTo(LifeTime);
        }

        #endregion
    }
}