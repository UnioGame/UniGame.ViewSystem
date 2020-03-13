using UniGreenModules.UniGame.UiSystem.Runtime;
using UnityEngine;

namespace UniGame.UiSystem.Examples.ListViews.Views
{
    using System.Collections.Generic;
    using Runtime;
    using UniRx;
    using UniRx.Async;
    using UnityEngine.UI;
    using ViewModels;

    public class DemoListView : WindowView<DemoListViewModel>
    {
        public RectTransform itemsParent;

        public Button addItem;
        
        public List<DemoItemView> itemViews = new List<DemoItemView>();

        protected override void OnWindowInitialize(DemoListViewModel model)
        {
            var items = model.ListItems;
            
            BindTo(items.ObserveAdd(), x => CreateItem(x.Value)).
            BindTo(items.ObserveRemove(), x => RemoveItem(x.Index)).
            BindTo(addItem.onClick.AsObservable(),x => model.Add.Execute());
        }

        private async UniTask<DemoItemView> CreateItem(DemoItemViewModel itemModel)
        {
            var view = await ViewFactory.Create<DemoItemView>(itemModel);
            view.transform.SetParent(itemsParent);
            itemViews.Add(view);
            LayoutRebuilder.MarkLayoutForRebuild(itemsParent);
            return view;
        }

        private void RemoveItem(int index)
        {
            var item = itemViews[index]; 
            itemViews.RemoveAt(index);
            item.Close();
        }
        
        
    }
}
