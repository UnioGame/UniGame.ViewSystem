namespace UniGreenModules.UniGame.UiSystem.Runtime.Abstracts
{
    using UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IView : ILifeTimeContext
    {
        IReadOnlyReactiveProperty<bool> IsActive { get; }

        // Вместо IViewElementFactory хочется передавать IContent с рестришкном на IViewService
        void Initialize(IViewModel vm,IViewElementFactory viewFactory);
        
        void Close();

        void Show();

        void Hide();

    }
}