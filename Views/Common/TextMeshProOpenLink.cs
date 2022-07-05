using UnityEngine.EventSystems;

namespace UniGame.UI.Components
{
    using TMPro;
    using UnityEngine;

    public class TextMeshProOpenLink : UIBehaviour,IPointerClickHandler 
    {

        public TextMeshProUGUI _text;
        
        public void OnPointerClick (PointerEventData eventData) {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink (_text, eventData.position, eventData.pressEventCamera);
            if (linkIndex == -1) 
                return;
            var linkInfo     = _text.textInfo.linkInfo[linkIndex];
            var       selectedLink = linkInfo.GetLinkID();
            
            if (string.IsNullOrEmpty(selectedLink))
                return;
            
            Debug.LogFormat ("Open link {0}", selectedLink);
            
            Application.OpenURL (selectedLink);
        }

        protected override void Awake()
        {
            _text ??= GetComponent<TextMeshProUGUI>();
        }
    }
}
