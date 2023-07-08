namespace Game.Modules.UnioModules.UniGame.ViewSystem.Runtime.Views.Abstract
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.Core.Runtime;
    using global::UniGame.ViewSystem.Runtime;

    public interface IViewAnimation
    {
        UniTask Show(IView view, ILifeTime lifeTime);
        UniTask Hide(IView view, ILifeTime lifeTime);
        UniTask Close(IView view, ILifeTime lifeTime);
    }
}