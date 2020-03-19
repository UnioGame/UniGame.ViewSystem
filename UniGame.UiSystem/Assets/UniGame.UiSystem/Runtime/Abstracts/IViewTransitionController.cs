namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UnityEngine.SceneManagement;

    // можно заменить абстрактным классом а биндинг к эвентам сцены переместить внутрь класса
    // чтобы не размазывать ответственность за вызов обработчиков и сами обработчики

    public interface IViewFlowController
    {
        void Activate(IViewLayoutContainer map);
        
        void OnSceneActivate(Scene current, Scene next);

        void OnSceneLoaded(Scene current, LoadSceneMode mode);

        void OnSceneUnloaded(Scene current);
    }
}