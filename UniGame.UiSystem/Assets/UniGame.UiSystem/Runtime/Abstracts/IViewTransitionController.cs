namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UnityEngine.SceneManagement;

    public interface IViewFlowController
    {
        void Activate(IViewLayoutContainer map);
        
        void OnSceneActivate(Scene current, Scene next);

        void OnSceneLoaded(Scene current, LoadSceneMode mode);

        void OnSceneUnloaded(Scene current);
    }
}