namespace UniGame.UI.Components
{
    using R3;
    using UniGame.UiSystem.Runtime;
    using Runtime.DataFlow;
    using ViewSystem.Runtime;
     
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class ViewCloseHandler : MonoBehaviour, 
        IPointerUpHandler,
        IPointerDownHandler, IPointerClickHandler
    {
        [SerializeField] private bool _closeOnMissclick = true;
        [SerializeField] private bool _closeOnBackButtonPressed = true;

        [SerializeField] private ViewBase _view;

        private LifeTime _lifeTime = new();
        
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
                          .Subscribe(this,static (x,y) => y.HandleBackButton())
                          .AddTo(_lifeTime);
            }
        }

        private void OnDisable() =>  _lifeTime.Restart();

        private void OnDestroy() => _lifeTime.Terminate();

        private void HandleBackButton()
        {
            if(Input.GetKeyDown(KeyCode.Escape) && 
               (_view.Status.CurrentValue == ViewStatus.Showing || 
                _view.Status.CurrentValue == ViewStatus.Shown))
                _view.Close();
        }
    }
}