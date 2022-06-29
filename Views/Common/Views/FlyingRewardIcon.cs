namespace Taktika.UI.Common.Views
{
    using UnityEngine;
    using UnityEngine.UI;

    public class FlyingRewardIcon : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void SetSprite(Sprite sprite)
        {
            _image.gameObject.SetActive(true);
            _image.sprite = sprite;
        }

        public void HideSprite()
        {
            _image.gameObject.SetActive(false);
        }
    }
}