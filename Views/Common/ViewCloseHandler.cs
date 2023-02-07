namespace UniGame.UI.Components
{
    using UniGame.UiSystem.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.UISystem.Runtime;
    using UniRx;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class ViewCloseHandler : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
    {
        [SerializeField] private bool _closeOnMissclick = true;
        [SerializeField] private bool _closeOnBackButtonPressed = true;

        [SerializeField] private ViewBase _view;

        private LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if(_closeOnMissclick)
                _view.Close();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }
        
        private void OnValidate()
        {
            if(_view == null)
                _view = GetComponentInParent<ViewBase>();

            if (TryGetComponent<Graphic>(out _)) return;
            
            var image = gameObject.AddComponent<Image>();
            image.color = Color.clear;
        }

        private void OnEnable()
        {
            if (_closeOnBackButtonPressed)
            {
                Observable.EveryUpdate()
                          .Subscribe(_ => HandleBackButton())
                          .AddTo(_lifeTimeDefinition);
            }
        }

        private void OnDisable()
        {
            _lifeTimeDefinition.Release();
        }

        private void OnDestroy()
        {
            _lifeTimeDefinition.Terminate();
        }

        private void HandleBackButton()
        {
            if(Input.GetKeyDown(KeyCode.Escape) && 
               (_view.Status.Value == ViewStatus.Showing || _view.Status.Value == ViewStatus.Shown))
                _view.Close();
        }
    }
}