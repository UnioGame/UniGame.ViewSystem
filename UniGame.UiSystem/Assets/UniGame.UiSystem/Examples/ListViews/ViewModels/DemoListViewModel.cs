using UniGreenModules.UniGame.UiSystem.Runtime;

namespace UniGreenModules.UniGame.UiSystem.Examples.ListViews.ViewModels
{
    using System;
    using UniRx;

    public class DemoListViewModel : ViewModelBase
    {
        public ReactiveCollection<DemoItemViewModel> ListItems = new ReactiveCollection<DemoItemViewModel>();
        
        public ReactiveCommand Add = new ReactiveCommand();
        
    }
}
