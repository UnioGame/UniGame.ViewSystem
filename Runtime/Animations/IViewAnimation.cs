namespace UniGame.ViewSystem.Runtime.Views.Abstract
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.Core.Runtime;
    using Runtime;

    public interface IViewAnimation
    {
        public bool IsEnabled { get; }
        UniTask PlayAnimation(IView view, ViewStatus status, ILifeTime lifeTime);
    }
}