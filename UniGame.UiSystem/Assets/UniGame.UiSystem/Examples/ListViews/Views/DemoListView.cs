using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
using UniGreenModules.UniGame.UiSystem.Examples.ListViews.ViewModels;
using UniGreenModules.UniGame.UiSystem.Runtime;
using UnityEngine;

namespace UniGreenModules.UniGame.UiSystem.Examples.ListViews.Views
{
    using System.Collections.Generic;
    using Runtime.Extensions;
    using UniRx;
    using UniRx.Async;
    using UnityEngine.UI;

    public class DemoListView : WindowView<DemoListViewModel>
    {
        public RectTransform itemsParent;

        public Button addItem;
        
        public List<DemoItemView> itemViews = new List<DemoItemView>();

        protected override void OnWindowInitialize(DemoListViewModel model, ILifeTime lifeTime)
        {
            var items = model.ListItems;
            
            this.
                Bind(items.ObserveAdd(), x => CreateItem(x.Value)).
                Bind(items.ObserveRemove(), x => RemoveItem(x.Index)).
                Bind(addItem.onClick.AsObservable(), model.Add);
        }

        private async UniTask<DemoItemView> CreateItem(DemoItemViewModel itemModel)
        {
            var view = await ViewFactory.Open<DemoItemView>(itemModel);
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
