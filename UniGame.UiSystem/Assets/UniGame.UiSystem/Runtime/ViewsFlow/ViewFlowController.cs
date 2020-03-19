namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UnityEngine.SceneManagement;

    // из названия класса не очевидно какое поведение он в себе содержит
    // Я бы сделал его абстрактным с подпиской на все события SceneManagement
    // а конкретное поведение обрабатывал бы в наследниках
    public class ViewFlowController : IViewFlowController
    {
        protected IViewLayoutContainer _controllerContainer;

        public void Activate(IViewLayoutContainer controllerContainer)
        {
            _controllerContainer = controllerContainer;
            OnActivate(controllerContainer);
        }

        public virtual void OnSceneActivate(Scene current, Scene next)
        {
            _controllerContainer[ViewType.Screen].CloseAll();
            _controllerContainer[ViewType.Window].CloseAll();
        }

        public virtual void OnSceneLoaded(Scene current, LoadSceneMode mode)
        {
            //no actions
        }

        public virtual void OnSceneUnloaded(Scene current)
        {
            //no actions
        }

        protected virtual void OnActivate(IViewLayoutContainer layouts)
        {
            
        }
        
    }
}