using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UniGreenModules.UniGame.UiSystem.Examples.BaseUiManager.Scripts
{
    using Runtime;
    using Runtime.Abstracts;
    using UniRx;
    using UniRx.Async;

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

            if (view.IsActive.Value) {
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
                    view = await uiViewManager.Open<DemoWindowView>(new ViewModelBase(),skin);
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
            text.text = view.IsActive.Value
                ? $"Hide {index}"
                : $"Show {index}";
        }
    }
}
