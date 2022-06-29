namespace Taktika.UI.Components
{
    using UniModules.UniCore.Runtime.Attributes;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Animator))]
    public class ButtonAnimatorController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField, ReadOnlyValue]
        private Button _button;
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private string _pressedTrigger = "Pressed";
        [SerializeField]
        private string _unpressedTrigger = "Unpressed";

        private int _pressedHash;
        private int _unpressedHash;
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _animator.SetTrigger(_unpressedHash);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _animator.SetTrigger(_pressedHash);
        }

        private void Awake()
        {
            _pressedHash = Animator.StringToHash(_pressedTrigger);
            _unpressedHash = Animator.StringToHash(_unpressedTrigger);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _button = GetComponent<Button>();
            _animator = GetComponent<Animator>();
        }
#endif
    }
}