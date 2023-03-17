using UnityEngine;

namespace UniGame.UI
{
    using TMPro;
    using UnityEngine.UI;

    public class TMProContraintLayoutElement : MonoBehaviour, ILayoutElement
    {
        [SerializeField] private int _layoutPriority = 10;
        [SerializeField] private float _minWidth = 0;
        [SerializeField] private float _minHeight = 0;
        [Space] 
        [SerializeField] private float _maxWidth = 100;
        [SerializeField] private float _maxHeight = 250;
        [SerializeField] private TextMeshProUGUI _textMesh;

        public void CalculateLayoutInputHorizontal()
        {
            // TODO
        }

        public void CalculateLayoutInputVertical()
        {
            // TODO
        }

        public void OnValidate()
        {
            if (_textMesh == null)
                _textMesh = GetComponent<TextMeshProUGUI>();
        }

        public float minWidth => _minWidth;
        public float preferredWidth => Mathf.Min(_textMesh.preferredWidth, _maxWidth);
        public float flexibleWidth => 1;
        public float minHeight => _minHeight;
        public float preferredHeight => Mathf.Min(_textMesh.preferredHeight, _maxHeight);
        public float flexibleHeight => 1;
        public int layoutPriority => _layoutPriority;
    }
}
