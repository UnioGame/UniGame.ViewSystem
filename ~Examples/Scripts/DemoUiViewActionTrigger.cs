using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UniGame.UiSystem.Examples.BaseUiManager.Scripts
{
    using System;
    using Cysharp.Threading.Tasks;
    using Runtime;
    using UniModules.UniGame.UiSystem.Examples.BaseUiManager;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    

    public enum DemoUiOperation
    {
        Show,
        Hide,
        Close,
    }
    
    public class DemoUiViewActionTrigger : MonoBehaviour
    {
        public TextMeshProUGUI status;

        public string skin;
        public DemoUiType type;
        public DemoWindowManager windowManager;

        public IView view;
        
        // Start is called before the first frame update
        private void Start()
        {
            status = status ?? GetComponentInChildren<TextMeshProUGUI>();
            windowManager = windowManager ?? GetComponentInParent<DemoWindowManager>();
            var buttons = GetComponentsInChildren<Button>();
            
            buttons[0].onClick.AsObservable().Subscribe(x => OnClick(DemoUiOperation.Show)).AddTo(this);
            buttons[1].onClick.AsObservable().Subscribe(x => OnClick(DemoUiOperation.Hide)).AddTo(this);
            buttons[2].onClick.AsObservable().Subscribe(x => OnClick(DemoUiOperation.Close)).AddTo(this);
            
        }

        public async UniTask OnClick(DemoUiOperation uiOperation)
        {
            if (uiOperation != DemoUiOperation.Show && view == null)
                return;
            
            if (view == null) {
                
                view = await Show();
                view.LifeTime.AddCleanUpAction(() => view = null);
                
            }

            switch (uiOperation) {
                case DemoUiOperation.Show:
                    view.Show();
                    break;
                case DemoUiOperation.Hide:
                    view.Hide();
                    break;
                case DemoUiOperation.Close:
                    view.Close();
                    view = null;
                    break;
            }
        }

        private async UniTask<IView> Show()
        {
            var uiViewManager = windowManager.uiViewManager;
            switch (type) {
                case DemoUiType.Element:
                    view = await uiViewManager.Create<DemoWindowView>(new ViewModelBase(),skin);
                    break;
                case DemoUiType.Screen:
                    view = await uiViewManager.OpenScreen<DemoScreenView>(new ViewModelBase(),skin);
                    break;
                case DemoUiType.Window:
                    view = await uiViewManager.OpenWindow<DemoWindowView>(new ViewModelBase(),skin);
                    break;
            }

            return view;
        }

        private void Update()
        {
            if (view == null) {
                status.text = "CLOSED";
                return;
            }

            if (view.IsVisible.Value) {
                status.text = "SHOWN";
            }
            else {
                status.text = "HIDDEN";
            }
        }
    }
}
