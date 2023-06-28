namespace UniGame.UI.Components
{
    using UnityEngine;
    using UnityEngine.UI;

    public class OpenLinkButton : MonoBehaviour
    {
        public string link;
        public Button button;
        
        public void Open()
        {
            if (string.IsNullOrEmpty(link)) return;
            Application.OpenURL (link);
        }

        private void Awake()
        {
            button ??= GetComponent<Button>();
            if (button == null) return;
            button.onClick.AddListener(Open);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Open);
        }
    }
}