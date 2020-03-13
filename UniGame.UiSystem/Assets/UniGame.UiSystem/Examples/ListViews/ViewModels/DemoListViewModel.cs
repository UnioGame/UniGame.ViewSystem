namespace UniGame.UiSystem.Examples.ListViews.ViewModels
{
    using Runtime;
    using UniRx;

    public class DemoListViewModel : ViewModelBase
    {
        public ReactiveCollection<DemoItemViewModel> ListItems = new ReactiveCollection<DemoItemViewModel>();
        
        public ReactiveCommand Add = new ReactiveCommand();
        
    }
}
