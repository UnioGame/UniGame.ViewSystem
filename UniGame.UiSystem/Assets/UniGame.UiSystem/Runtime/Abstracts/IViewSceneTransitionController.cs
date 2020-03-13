namespace UniGame.UiSystem.Runtime
{
    using UnityEngine.SceneManagement;

    public interface IViewSceneTransitionController
    {
        void OnSceneActivate(Scene current, Scene next);

        void OnSceneLoaded(Scene current, LoadSceneMode mode);

        void OnSceneUnloaded(Scene current);
    }
}