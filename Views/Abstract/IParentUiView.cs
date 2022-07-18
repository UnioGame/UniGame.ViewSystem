namespace UniGame.UI.Common.Abstract
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UnityEngine;

    public interface IParentUiView
    {
        UniTask<TView> CreateChildView<TView>(IViewModel viewModel) where TView : class, IView;
                
        UniTask<TView> OpenChildView<TView>(IViewModel viewModel) where TView : class, IView;

        TView GetChildView<TView>() where TView : Component, IView;

        void AddChildView(IView view);
    }
}