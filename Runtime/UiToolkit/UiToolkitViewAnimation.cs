namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime;
    using UniGame.UiSystem.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UnityEngine.UIElements;
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
            var presentation = view?.GameObject?.GetComponent<UiToolkitViewPresentation>();
            var root = presentation?.Root;
            if (root == null)
                return UniTask.CompletedTask;

            root.style.opacity = status == ViewStatus.Hidden || status == ViewStatus.Closed ? 0f : 1f;
            return UniTask.CompletedTask;
        }
    }
}
