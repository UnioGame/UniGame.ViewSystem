using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UniGame.UiSystem.Examples.BaseUiManager.Scripts
{
    using Cysharp.Threading.Tasks;
    using Runtime;
    using UniModules.UniGame.UiSystem.Examples.BaseUiManager;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    

    public class DemoUiViewTrigger : MonoBehaviour
    {
        public Button          buttonTrigger;
        public TextMeshProUGUI text;

        public string skin;
        public DemoUiType type;
        
        public DemoWindowManager windowManager;

        public IView view;
        
        // Start is called before the first frame update
        private void Start()
        {
            buttonTrigger = buttonTrigger ?? GetComponent<Button>();
            text = text ?? GetComponentInChildren<TextMeshProUGUI>();
            windowManager = windowManager ?? GetComponentInParent<DemoWindowManager>();
            
            buttonTrigger.onClick.
                AsObservable().
                Subscribe(x => OnClick()).
                AddTo(this);
        }

        private async UniTask OnClick()
        {
            if (view == null) {
                
                view = await Show();
                view.LifeTime.AddCleanUpAction(() => view = null);
                
                return;
            }

            if (view.IsVisible.Value) {
                view.Hide();
            }
            else {
                view.Show();
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
            var index = transform.GetSiblingIndex();
            if (view == null) {
                text.text = $"Open {index}";
                return;
            }
            text.text = view.IsVisible.Value
                ? $"Hide {index}"
                : $"Show {index}";
        }
    }
}
