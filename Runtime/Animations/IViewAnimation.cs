namespace UniGame.ViewSystem.Runtime.Views.Abstract
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.Core.Runtime;
    using Runtime;
    using UniModules.UniGame.UISystem.Runtime;

    public interface IViewAnimation
    {
        UniTask PlayAnimation(IView view, ViewStatus status, ILifeTime lifeTime);
    }
}