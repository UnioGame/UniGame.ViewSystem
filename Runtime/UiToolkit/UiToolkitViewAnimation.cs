namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime;
    using UniGame.UiSystem.Runtime;
    using UniGame.ViewSystem.Runtime;
    using ViewSystem.Runtime.Views.Abstract;

    /// <summary>
    /// IViewAnimation adapter for UITK. It uses the existing ViewSystem
    /// progress lifetime and does not add a second animation dependency.
    /// </summary>
    public sealed class UiToolkitViewAnimation : IViewAnimation
    {
        public bool IsEnabled => true;

        public UniTask PlayAnimation(IView view, ViewStatus status, ILifeTime lifeTime)
        {
            if (view is IUiToolkitView toolkit)
                toolkit.SetVisible(status is ViewStatus.Showing or ViewStatus.Shown);
            return UniTask.CompletedTask;
        }
    }
}
