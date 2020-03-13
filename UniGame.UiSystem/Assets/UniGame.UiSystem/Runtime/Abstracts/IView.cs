namespace UniGame.UiSystem.Runtime.Abstracts
{
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IView : ILifeTimeContext
    {
        IReadOnlyReactiveProperty<bool> IsActive { get; }
        
        void Initialize(IViewModel vm,IViewElementFactory viewFactory);
        
        void Close();

        void Show();

        void Hide();

    }
}