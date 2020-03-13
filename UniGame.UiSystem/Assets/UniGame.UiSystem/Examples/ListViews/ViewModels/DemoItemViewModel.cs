namespace UniGame.UiSystem.Examples.ListViews.ViewModels
{
    using System;
    using Runtime;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class DemoItemViewModel : ViewModelBase
    {
        public IntReactiveProperty Damage = new IntReactiveProperty(0);
        public IntReactiveProperty Level = new IntReactiveProperty(0);
        public IntReactiveProperty Armor = new IntReactiveProperty(0);
        public IntReactiveProperty Cost = new IntReactiveProperty(0);
        public ReactiveProperty<Sprite> Icon = new ReactiveProperty<Sprite>();
        
        public ReactiveCommand Sell = new ReactiveCommand();
        public ReactiveCommand Remove = new ReactiveCommand();
    }
}
