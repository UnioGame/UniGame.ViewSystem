using System.Collections.Generic;
using UnityEngine;

namespace Taktika.Lobby.Runtime.UI.Views
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Sirenix.OdinInspector;
    using TMPro;
    using UniGame.UiSystem.Runtime;
    using UniModules.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataStructure;
    using UniModules.UniGame.Core.Runtime.Extension;
    using UnityEngine.UI;

    public class ArrowHintView : UiView<IArrowHintViewModel>
    {
        [SerializeField] protected Vector3 BorderOffset = new Vector3(10, 10);
        [SerializeField] protected TextMeshProUGUI Text;
        [SerializeField] protected RectTransform TextParent;
        
        [DrawWithUnity]
        [SerializeField] protected List<ArrowData> Arrows;
        
        [SerializeField] private Canvas _selfCanvas;
        [SerializeField] private bool _fixPositionByContainer = false;

        [SerializeField] private SerializableDictionary<ArrowSide, float> _additionOffsets = new SerializableDictionary<ArrowSide, float>();
        
        protected ArrowSide ArrowSide;
        
        private const int BottomLeftIndex = 0;
        private const int TopRightIndex = 2;

        private Vector3 _userCustomOffset;
        

        public void PointOnUi(RectTransform target, ArrowDirection? arrowPosition = null, ArrowSide arrowSide = ArrowSide.Auto, Vector3 customOffset = new Vector3())
        {
            var corners = new Vector3[4];
            target.GetWorldCorners(corners);
            PointOn(corners, arrowPosition, arrowSide, customOffset);
        }

        public void PointOn(Vector3[] screenWorldCorners, ArrowDirection? arrowPosition = null, ArrowSide arrowSide = ArrowSide.Auto, Vector3 customOffset = new Vector3())
        {
            ArrowSide         = arrowSide;
            _userCustomOffset = customOffset;
            
            if (!arrowPosition.HasValue)
                arrowPosition = EvaluateArrowDirection(screenWorldCorners);

            foreach (var arrow in Arrows)
            {
                var active = arrow.Direction == arrowPosition;
                arrow.SetActive(active);
                
                arrow.GetRectTransform().localPosition = Vector3.zero;
            }
            
            if (ArrowSide == ArrowSide.Auto)
                ArrowSide = GetArrowSide(arrowPosition.Value);
            
            var corners = new Vector3[4];
            var thisRectTransform = this.transform as RectTransform;
            var targetPivotPoint = GetTargetAnchorPoint(screenWorldCorners, arrowPosition.Value);
            
            thisRectTransform.GetWorldCorners(corners);
            
            var selfWorldPivot = GetSelfAnchorPoint(corners, arrowPosition.Value);
            var delta = targetPivotPoint - selfWorldPivot;
            var startPos = thisRectTransform.position;
            var endPos = startPos + delta;
            
            thisRectTransform.position = endPos;
            
            SetTextAlignment(arrowPosition.Value);
            FixTextRectPosition();
        }

        public void DisableArrows() 
        {
            foreach (var arrow in Arrows)
                arrow.SetActive(false);
        }

        public void SetSortingOverride(int sortingOrder)
        {
            _selfCanvas.overrideSorting = true;
            _selfCanvas.sortingOrder = sortingOrder;
        }

        protected override async UniTask OnInitialize(IArrowHintViewModel model)
        {
            await base.OnInitialize(model);
            
            foreach(var arrow in Arrows)
                arrow.SetActive(false);
            
            this.Bind(model.Text, s => TextParent.gameObject.SetActive(!string.IsNullOrEmpty(s)))
                .Bind(model.Text, s => { Text.text = s; FixTextRectPosition(); });
        }
        
        protected virtual void FixTextRectPosition()
        {
            // TODO подход с вычисением в мировых координатах работает только для overlay канваса
            // при переходе к камерам нужно будет переделать
            var corners = new Vector3[4];
            
            TextParent.localPosition = Vector3.zero;
            Text.ForceMeshUpdate(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(Text.rectTransform);
            
            if (_fixPositionByContainer)
                TextParent.GetWorldCorners(corners);
            else 
                Text.RectTransform().GetWorldCorners(corners);

            AlignBasedOnArrowSide(ArrowSide, corners);

            if (_fixPositionByContainer)
                TextParent.GetWorldCorners(corners);
            else 
                Text.RectTransform().GetWorldCorners(corners);
            
            EnsureTextContainerIsWithinScreenBounds(corners);
        }

        protected void EnsureTextContainerIsWithinScreenBounds(Vector3[] corners)
        {
            var rectTopRight   = corners[TopRightIndex];
            var rectBottomLeft = corners[BottomLeftIndex];

            var screenTopRight   = new Vector3(Screen.width, Screen.height) - BorderOffset;
            var screenBottomLeft = BorderOffset;

            var topRightDelta  = rectTopRight - screenTopRight;
            var topRightOffset = new Vector3(-Mathf.Max(0, topRightDelta.x), -Mathf.Max(0, topRightDelta.y));

            TextParent.position += topRightOffset;

            var bottomLeftDelta  = rectBottomLeft - screenBottomLeft;
            var bottomLeftOffset = new Vector3(-Mathf.Min(0, bottomLeftDelta.x), -Mathf.Min(0, bottomLeftDelta.y));

            TextParent.position += bottomLeftOffset;
        }

        protected virtual void SetTextAlignment(ArrowDirection arrowDirection)
        {
            switch (arrowDirection)
            {
                case ArrowDirection.Up:
                    Text.alignment = TextAlignmentOptions.Top;
                    break;
                case ArrowDirection.Down:
                    Text.alignment = TextAlignmentOptions.Bottom;
                    break;
                case ArrowDirection.Left:
                    Text.alignment = TextAlignmentOptions.Left;
                    break;
                case ArrowDirection.Right:
                    Text.alignment = TextAlignmentOptions.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arrowDirection), arrowDirection, null);
            }
        }
        
        private void AlignBasedOnArrowSide(ArrowSide arrowSide, Vector3[] cornersArray)
        {
            var activeArrow = Arrows.FirstOrDefault(x => x.IsActive);
            
            if(activeArrow == null)
                return;
            
            var arrow        = activeArrow;
            var arrowCorners = new Vector3[4];

            arrow.GetRectTransform().GetWorldCorners(arrowCorners);
            
            var containerBottomLeft = cornersArray[BottomLeftIndex];
            var containerTopRight = cornersArray[TopRightIndex];

            var containerWidth = containerTopRight.x - containerBottomLeft.x;
            var containerHeight = containerTopRight.y - containerBottomLeft.y;
            
            var referenceSide = GetReferenceSide(arrowSide, arrowCorners);

            TextParent.position =  GetTargetPosition(arrowSide, arrowCorners, referenceSide, containerHeight, containerWidth);
        }

        private float GetReferenceSide(ArrowSide arrowSide, Vector3[] arrowCorners)
        {
            return arrowSide switch
            {
                ArrowSide.Up => arrowCorners.Min(corner => corner.y),
                ArrowSide.Right => arrowCorners.Min(corner => corner.x),
                ArrowSide.Bottom => arrowCorners.Max(corner => corner.y),
                ArrowSide.Left => arrowCorners.Max(corner => corner.x),
                _ => 0
            };
        }

        private Vector3 GetTargetPosition(ArrowSide arrowSide, Vector3[] arrowCorners, float referenceSide, float containerHeight, float containerWidth)
        {
            var arrowCenter = Vector2.Lerp(arrowCorners[BottomLeftIndex], arrowCorners[TopRightIndex], .5f);
            var offsetFromArrow = GetOffsetFromArrow(arrowSide);

            return arrowSide switch
            {
                ArrowSide.Up => new Vector3(arrowCenter.x, referenceSide - containerHeight / 2f - offsetFromArrow, 0),
                ArrowSide.Right => new Vector3(referenceSide - containerWidth / 2f - offsetFromArrow, arrowCenter.y, 0),
                ArrowSide.Bottom => new Vector3(arrowCenter.x, offsetFromArrow + referenceSide + containerHeight / 2f, 0),
                ArrowSide.Left => new Vector3(offsetFromArrow + referenceSide + containerWidth / 2f, arrowCenter.y, 0),
                _ => Vector3.zero
            };
        }

        private float GetOffsetFromArrow(ArrowSide arrowSide)
        {
            if (!_additionOffsets.TryGetValue(arrowSide, out var offsetPercentage))
                offsetPercentage = 0;
            
            float additionalOffset;

            if (arrowSide == ArrowSide.Bottom || arrowSide == ArrowSide.Up)
                additionalOffset = Screen.height * offsetPercentage;
            else
                additionalOffset = Screen.width * offsetPercentage;

            return additionalOffset;
        }

        private ArrowDirection EvaluateArrowDirection(Vector3[] targetCorners)
        {
            var screenCenter = new Vector3(Screen.width / 2, Screen.height / 2);
            Vector3 cornersCenter = (targetCorners[0] + targetCorners[2]) / 2;
            return cornersCenter.y > screenCenter.y ? ArrowDirection.Up : ArrowDirection.Down;
        }

        private Vector3 GetTargetAnchorPoint(Vector3[] worldCorners, ArrowDirection direction)
        {
            switch (direction)
            {
                case ArrowDirection.Up:
                    return GetCenter(worldCorners[0], worldCorners[3]);
                case ArrowDirection.Down:
                    return GetCenter(worldCorners[1], worldCorners[2]);
                case ArrowDirection.Left:
                    return GetCenter(worldCorners[2], worldCorners[3]);
                case ArrowDirection.Right:
                    return GetCenter(worldCorners[1], worldCorners[0]);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private Vector3 GetSelfAnchorPoint(Vector3[] worldCorners, ArrowDirection direction)
        {
            switch (direction)
            {
                case ArrowDirection.Up:
                    return GetCenter(worldCorners[1], worldCorners[2]);
                case ArrowDirection.Down:
                    return GetCenter(worldCorners[0], worldCorners[3]);
                case ArrowDirection.Left:
                    return GetCenter(worldCorners[0], worldCorners[1]);
                case ArrowDirection.Right:
                    return GetCenter(worldCorners[2], worldCorners[3]);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private Vector3 GetCenter(Vector3 start, Vector3 end)
        {
            return Vector3.Lerp(start, end, 0.5f);
        }
        
        private ArrowSide GetArrowSide(ArrowDirection arrowDirection)
        {
            return arrowDirection switch
            {
                ArrowDirection.Down => ArrowSide.Bottom,
                ArrowDirection.Up => ArrowSide.Up,
                ArrowDirection.Left => ArrowSide.Left,
                ArrowDirection.Right => ArrowSide.Right,
                _ => ArrowSide.Bottom
            };
        }
    }

    [Serializable]
    public enum ArrowSide
    {
        Auto = 0,
        Up,
        Right,
        Bottom,
        Left
    }

    [Serializable]
    public enum ArrowDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [Serializable]
    public class ArrowData
    {
        [SerializeField] private GameObject _arrow;
        [SerializeField] private GameObject _arrowContainer;
        [SerializeField] private ArrowDirection _direction;

        public ArrowDirection Direction => _direction;

        public bool IsActive => _arrow.gameObject.activeInHierarchy;

        public void SetActive(bool value)
        {
            _arrowContainer.SetActive(value);
        }

        public RectTransform GetRectTransform()
        {
            return _arrow.transform as RectTransform;
        }

        public RectTransform GetArrowParentTransform()
        {
            return _arrowContainer.transform as RectTransform;
        }
    }
}
