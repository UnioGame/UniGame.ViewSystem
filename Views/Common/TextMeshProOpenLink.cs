using UnityEngine.EventSystems;

namespace UniGame.UI.Components
{
    using TMPro;
    using UniCore.Runtime.ProfilerTools;
    using UnityEngine;

    public class TextMeshProOpenLink : UIBehaviour, IPointerClickHandler
    {
        public TextMeshProUGUI _text;

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities
                .FindIntersectingLink(_text, eventData.position, eventData.pressEventCamera);
            
            if (linkIndex == -1) return;

            var linkInfo = _text.textInfo.linkInfo[linkIndex];
            var selectedLink = linkInfo.GetLinkID();

            if (string.IsNullOrEmpty(selectedLink))
                return;

            GameLog.Log($"Open link {selectedLink}",Color.blue);

            Application.OpenURL(selectedLink);
        }

        protected override void Awake()
        {
            _text ??= GetComponent<TextMeshProUGUI>();
        }
    }
}