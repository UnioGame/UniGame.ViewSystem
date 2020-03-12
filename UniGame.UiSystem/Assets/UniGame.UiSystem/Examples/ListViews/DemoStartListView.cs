using UnityEngine;

namespace UniGame.UiSystem.Examples.ListViews
{
    using global::UniGame.UiSystem.Runtime;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.UiSystem.Examples.BaseUiManager.Scripts;
    using UniGreenModules.UniGame.UiSystem.Examples.ListViews.ViewModels;
    using UniGreenModules.UniGame.UiSystem.Examples.ListViews.Views;
    using UniGreenModules.UniGame.UiSystem.Runtime.Extensions;
    using UniRx;

    public class DemoStartListView : MonoBehaviour
    {
        public GameViewSystemAsset viewSystem;
    
        public DemoItemViewModel demoItemViewModel = new DemoItemViewModel();
    
        public DemoListViewModel listModel = new DemoListViewModel();

        public DemoResourceUiViewModel resourceModel = new DemoResourceUiViewModel();
        
        public Sprite[] icons;

        public int itemsCounter;
        
        // Start is called before the first frame update
        private async void Start()
        {
            var view = await viewSystem.OpenScreen<DemoListView>(listModel);
            var resourcePanel = await viewSystem.OpenScreen<DemoResourcePanelScreenView>(resourceModel);

            listModel.AddTo(this);
            resourceModel.AddTo(this);

            listModel.Add.Do(x => GameLog.Log($"ADD NEW Demo List Item")).
                Do(x => this.itemsCounter++).
                Subscribe(x =>CreateItemViewModel()).
                AddTo(this);

            listModel.ListItems.ObserveRemove().
                Do(x => x.Value.Dispose()).
                Subscribe().
                AddTo(this);
            
            view.Show();
            resourcePanel.Show();
            
        }

        private void CreateItemViewModel()
        {
            var armor = demoItemViewModel.Armor.Value + itemsCounter;
            var damage = demoItemViewModel.Damage.Value + itemsCounter;
            var level = demoItemViewModel.Level.Value + itemsCounter;
            var cost = (armor + damage) + level * 100;
            
            var model = new DemoItemViewModel() {
                Armor  = new IntReactiveProperty(armor),
                Damage = new IntReactiveProperty(damage),
                Level  = new IntReactiveProperty(level),
                Cost  = new IntReactiveProperty(cost),
                Icon   = new ReactiveProperty<Sprite>(icons.Length == 0 ? null : icons[itemsCounter % icons.Length]),
                Sell    = new ReactiveCommand(),
                Remove = new ReactiveCommand(),
            }; 

            model.Bind(model.Sell, x => {
                resourceModel.Gold.Value += model.Cost.Value;
                listModel.ListItems.Remove(model);
            });
            
            model.Bind(model.Remove, x => listModel.ListItems.Remove(model));
            
            listModel.ListItems.Add(model);
        }
    }
}
