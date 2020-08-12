namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniRx;
    using UnityEngine.SceneManagement;
    
    public class ViewFlowController : IViewFlowController
    {
        private readonly LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();
        protected IViewLayoutContainer _controllerContainer;

        public ILifeTime LifeTime => _lifeTimeDefinition;
        
        public void Activate(IViewLayoutContainer controllerContainer)
        {
            _lifeTimeDefinition.Release();
            _lifeTimeDefinition.AddCleanUpAction(() => _controllerContainer = null);

            _controllerContainer = controllerContainer;
            
            BindSceneActions();
            
            OnActivate(controllerContainer);
        }

        public void Dispose() => _lifeTimeDefinition.Terminate();
        
        #region peivate methods
        
        protected virtual void OnSceneActivate(Scene current, Scene next)
        {
            _controllerContainer.GetLayout(ViewType.Screen).CloseAll();
            _controllerContainer.GetLayout(ViewType.Window).CloseAll();
        }

        protected virtual void OnSceneLoaded(Scene current, LoadSceneMode mode)
        {
            //no actions
        }

        protected virtual void OnSceneUnloaded(Scene current)
        {
            //no actions
        }

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