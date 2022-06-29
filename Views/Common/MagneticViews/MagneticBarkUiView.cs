namespace Taktika.Lobby.Runtime.UI.Views
{
    using System.Collections;
    using TMPro;
    using UniModules.UniRoutine.Runtime;
    using UniModules.UniRoutine.Runtime.Extension;
    using UniModules.UniGame.Core.Runtime.Extension;
    using UnityEngine.UI;
    using PixelCrushers.DialogueSystem;
    using Taktika.UI.Common.MagneticViews.Abstract;
    using UnityEngine;

    public class MagneticBarkUiView : BaseFollowingUI<BaseFollowingViewModel>, IFollowingBarkUI
    {
        [SerializeField] private RectTransform _tailTransform;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private int _linesForSideTail;

        // HACK - DialogueActor не работает с интерфейсом IBarkUI, 
        // TODO возможно следует использовать явную реализацию интерфейса

        private BarkUiMediatorComponent _mediatorComponent;
        private float _doneTime;

        private RoutineHandle _hideRoutine;

        protected override void Awake()
        {
            _mediatorComponent = gameObject.AddComponent<BarkUiMediatorComponent>();
            _mediatorComponent.SetTargetObject(this);
        }

        public static implicit operator AbstractBarkUI(MagneticBarkUiView view) => view._mediatorComponent;

        public bool isPlaying => _doneTime >= DialogueTime.time;
        
        public void Bark(Subtitle subtitle)
        {
            _hideRoutine.Cancel();
            if (string.IsNullOrEmpty(subtitle.formattedText.text))
            {
                Hide();
                return;
            }
            _text.text = subtitle.formattedText.text;
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.RectTransform());
            Show();
            // HACK
            var duration = DialogueManager.instance.GetBarkDuration(subtitle.formattedText.text);
            _doneTime = DialogueTime.time + duration;
            _hideRoutine = HideRoutine(duration).Execute().AddTo(LifeTime);
        }

        private IEnumerator HideRoutine(float waitDuration)
        {
            yield return this.WaitForSeconds(waitDuration);
            Hide();
        }

        public override void Init(RectTransform bounds, IFollowable target)
        {
            base.Init(bounds, target);
            RectTransform.pivot = Vector2.right;
        }

        protected override void SetPosition()
        {
            if (Target == null)
            {
                return;
            }
            
            var canvasPos = GetBarkBaseOnCanvas(true);
            SetBestPivot(canvasPos);
            var offset = GetBarkOffset(canvasPos);
            RectTransform.anchoredPosition = canvasPos + offset;
            SetTailTransform( offset);
            var tailOffset = GetTailOffset(canvasPos);
            RectTransform.anchoredPosition += new Vector2(tailOffset.x, tailOffset.y);
        }

        private void SetBestPivot(Vector3 barkBasePos)
        {
            var rect = this.RectTransform();
            var rectSize = rect.sizeDelta;
            var parentSize = Bounds.sizeDelta;

            var areaDivider = new Vector2(rectSize.x / 2, parentSize.y - rectSize.y / 2);
            rect.pivot = new Vector2(barkBasePos.x > areaDivider.x ? 1 : 0, barkBasePos.y < areaDivider.y ? 0 : 1);
        }

        private Vector3 GetBarkOffset(Vector3 barkBasePos)
        {
            var rect = this.RectTransform();
            var rectSize = rect.sizeDelta;
            var parentSize = Bounds.sizeDelta;

            var offset = new Vector3
            {
                y = rect.pivot.y > 0.5f ? parentSize.y - barkBasePos.y : -Mathf.Max(barkBasePos.y + rectSize.y - parentSize.y, 0),
                x = rect.pivot.x < 0.5f ? -barkBasePos.x : -Mathf.Min(barkBasePos.x - rectSize.x, 0)
            };

            return offset;
        }

        private void SetTailTransform(Vector3 offset)
        {
            var rect = this.RectTransform();
            var pivot = rect.pivot;
            
            var isMainOffsetX = Mathf.Abs(offset.x / RectTransform.sizeDelta.x) >= Mathf.Abs(offset.y / RectTransform.sizeDelta.y);

            var tailAnchoredPosition = new Vector2(0, -_tailTransform.rect.height);
            _tailTransform.localRotation = Quaternion.identity;
            if (pivot.x < 0.5)
            {
                _tailTransform.RotateAround(_tailTransform.pivot, Vector3.up, 180);
            }
            if (pivot.y > 0.5)
            {
                _tailTransform.RotateAround(_tailTransform.pivot, Vector3.right, 180);
                tailAnchoredPosition *= -1;

            }
            if (!isMainOffsetX && _text.textInfo.lineCount >= _linesForSideTail)
            {
                var yDirection = pivot.y <= 0.5f ? (pivot.x < 0.5f ? 1 : -1) : (pivot.x < 0.5f ? -1 : 1);
                _tailTransform.RotateAround(_tailTransform.pivot, new Vector3(1, yDirection, 0), 180);
                tailAnchoredPosition = new Vector2(tailAnchoredPosition.y, tailAnchoredPosition.x);
                tailAnchoredPosition *= yDirection;
            }

            var tailAnchor = GetTailAnchor(isMainOffsetX, offset);

            _tailTransform.anchorMin = _tailTransform.anchorMax = tailAnchor;
            _tailTransform.anchoredPosition = tailAnchoredPosition;
        }

        private Vector2 GetTailAnchor(bool isMainOffsetX, Vector3 offset)
        {
            var rect = this.RectTransform();
            var rectSize = rect.sizeDelta;
            var pivot = rect.pivot;

            var tailAnchor = new Vector2();
            tailAnchor.x = !isMainOffsetX ? Mathf.Round(pivot.x) :
                (pivot.x < 0.5f ?
                    1 - Mathf.Clamp01((rectSize.x + offset.x) / rectSize.x) :
                        Mathf.Clamp01((rectSize.x - offset.x) / rectSize.x));
            tailAnchor.y = (isMainOffsetX || _text.textInfo.lineCount < _linesForSideTail) ? Mathf.Round(pivot.y) :
                (pivot.y > 0.5f ?
                    1 - Mathf.Clamp01(offset.y / rectSize.y) / 2 :
                        Mathf.Clamp01(-offset.y / rectSize.y) / 2);

            return tailAnchor;
        }

        private Vector3 GetTailOffset(Vector3 barkBasePos)
        {
            var rect = this.RectTransform();
            var rectSize = rect.sizeDelta;
            var pivot = rect.pivot;
            var parentSize = Bounds.sizeDelta;

            var tailScreenPos = _tailTransform.TransformPoint(_tailTransform.pivot);
            var tailCanvasPos = tailScreenPos * (1f / Bounds.localScale.x);
            var tailScreenOffset = new Vector3();
            if (_text.textInfo.lineCount < _linesForSideTail)
            {
                tailScreenOffset.y = pivot.y > 0.5f ? 0 : -Mathf.Max(barkBasePos.y + rectSize.y - parentSize.y, 0);
            }
            return barkBasePos - tailCanvasPos + tailScreenOffset;
        }
    }
}

