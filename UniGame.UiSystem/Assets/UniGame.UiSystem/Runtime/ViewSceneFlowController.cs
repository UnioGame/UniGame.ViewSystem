namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UnityEngine.SceneManagement;

    public class ViewSceneFlowController : IViewSceneTransitionController
    {
        private readonly IViewLayoutContainer _controllerContainer;

        public ViewSceneFlowController(IViewLayoutContainer controllerContainer)
        {
            _controllerContainer = controllerContainer;
        }

        public void OnSceneActivate(Scene current, Scene next)
        {
            _controllerContainer[ViewType.Screen].CloseAll();
            _controllerContainer[ViewType.Window].CloseAll();
        }

        public void OnSceneLoaded(Scene current, LoadSceneMode mode)
        {
            //no actions
        }

        public void OnSceneUnloaded(Scene current)
        {
            //no actions
        }
    }
}