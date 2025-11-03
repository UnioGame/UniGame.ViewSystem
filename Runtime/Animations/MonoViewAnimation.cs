namespace UniGame.UiSystem.Runtime
{
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using ViewSystem.Runtime.Views.Abstract;
    using UnityEngine;
    using ViewSystem.Runtime;

    public class MonoViewAnimation : MonoBehaviour, IViewAnimation
    {
        public bool isEnabled = false;
        public bool IsEnabled => isEnabled;
        
        public UniTask PlayAnimation(IView view, ViewStatus status,ILifeTime lifeTime)
        {
            return UniTask.CompletedTask;
        }
        
        public virtual UniTask Show(IView view, ILifeTime lifeTime)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask Hide(IView view, ILifeTime lifeTime)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask Close(IView view, ILifeTime lifeTime)
        {
            return UniTask.CompletedTask;
        }

    }
}