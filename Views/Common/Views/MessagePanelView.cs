namespace Taktika.UI.Common.Views
{
    using System;
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using TMPro;
    using UniGame.UiSystem.Runtime;
    using UniModules.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.DoTweenRoutines.Runtime;
    using UniRx;
    using UnityEngine;
    using ViewModels.Abstract;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataStructure;
    using UnityEngine.UI;
    using ViewModels;

    public class MessagePanelView : UiView<IMessagePanelViewModel>
    {
        [SerializeField] private TMP_Text _messageText;

        [SerializeField] private RectTransform _messageTextRect;

        [Range(0, .2f)]
        [SerializeField] private float _screenEdgesOffsetPercent = .02f;
        
        [Header("Tips")]
        
        [SerializeField] private SerializableDictionary<MessagePanelSide, RectTransform> _tips = new SerializableDictionary<MessagePanelSide, RectTransform>(); 
        
        [Header("Animation Settings")]
        
        [SerializeField, Range(0.0f, 1.0f)] private float _duration = 0.3f;
        [SerializeField, Range(0.0f, 4.0f)] private float _delay = 2f;

        private Vector3[] _corners = new Vector3[4];
        private Vector2 _screenEdgesOffset;
        private MessagePanelSide _side;

        private float _canvasScaleFactor;
        
        private const int BottomLeftCornerIndex = 0;
        private const int TopRightCornerIndex = 2;

        protected override void OnAwake()
        {
            base.OnAwake();

            var parentCanvas = GetComponentInParent<Canvas>();
            _canvasScaleFactor = parentCanvas == null ? 1 : parentCanvas.scaleFactor;
        }

        protected override async UniTask OnInitialize(IMessagePanelViewModel model)
        {
            await base.OnInitialize(model);

            CanvasGroup.alpha = 0;
            _screenEdgesOffset = new Vector2(Screen.width * _screenEdgesOffsetPercent, Screen.height * _screenEdgesOffsetPercent);
            _side = Model.Side;

            _messageTextRect.pivot = Vector2.one * .5f;

            this.Bind(Model.MessageText, UpdateText);
            SetMessagePosition(Model.MessageOrigin, Model.OriginOffset);
        }

        protected override IEnumerator OnShowProgress(ILifeTime progressLifeTime)
        {
            yield return GetFadeSequence(1).WaitForCompletionTween();

            CanvasGroup.interactable   = true;
            CanvasGroup.blocksRaycasts = true;

            Observable.Timer(TimeSpan.FromSeconds(_delay)).Subscribe(_ => Close()).AddTo(ModelLifeTime);
        }

        protected override IEnumerator OnCloseProgress(ILifeTime progressLifeTime)
        {
            yield return GetFadeSequence(0).WaitForCompletionTween();
        }

        private void UpdateText(string newText)
        {
            _messageText.text = newText;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_messageTextRect);
        }

        private Sequence GetFadeSequence(float endValue)
        {
            CanvasGroup.alpha = 1 - endValue;

            var sequence  = DOTween.Sequence();
            var fadeTween = CanvasGroup.DOFade(endValue, _duration);

            sequence.Join(fadeTween);

            return sequence;
        }

        private void SetMessagePosition(Vector2 messageOrigin, Vector2 offset)
        {
            var screenBounds = GetScreenBounds();
            
            _messageTextRect.GetWorldCorners(_corners);
            
            var fullWidth = (_corners[TopRightCornerIndex].x - _corners[BottomLeftCornerIndex].x);
            var fullHeight = (_corners[TopRightCornerIndex].y - _corners[BottomLeftCornerIndex].y);

            var halfWidth = fullWidth * .5f;
            var halfHeight = fullHeight * .5f;
            
            var targetPosition = messageOrigin + offset;

            var additionalOffset = Vector2.zero;
            
            if (targetPosition.x + halfWidth > screenBounds.max.x)
            {
                additionalOffset.x = screenBounds.max.x - (targetPosition.x + halfWidth);
            }
            else if (targetPosition.x - halfHeight < screenBounds.min.x)
            {
                additionalOffset.x = screenBounds.min.x - (targetPosition.x - halfWidth);
            }

            if (targetPosition.y + halfHeight > screenBounds.max.y)
            {
                additionalOffset.y = screenBounds.max.y - (targetPosition.y + halfHeight);
            }
            else if (targetPosition.y - halfHeight < screenBounds.min.y)
            {
                additionalOffset.y = screenBounds.min.y - (targetPosition.y - halfHeight);
            }

            targetPosition += additionalOffset;

            _messageTextRect.position = targetPosition;

            UpdatePivot(fullWidth, fullHeight);
            UpdateTipBasedOnMessageSide(_side, fullWidth, fullHeight);
        }

        private (Vector2 min, Vector2 max) GetScreenBounds()
        {
            var screenBoundsMin = new Vector2
            (
                Screen.safeArea.xMin + _screenEdgesOffset.x, 
                Screen.safeArea.yMin + _screenEdgesOffset.y
            );

            var screenBoundsMax = new Vector2
            (
                Screen.width - Screen.safeArea.xMin - _screenEdgesOffset.y,
                Screen.height - Screen.safeArea.yMin - _screenEdgesOffset.y
            );

            return (screenBoundsMin, screenBoundsMax);
        }

        private void UpdateTipBasedOnMessageSide(MessagePanelSide messageSide, float fullWidth, float fullHeight)
        {
            var tipSide = GetTipSideFromMessageSide(messageSide);

            foreach (var pair in _tips)
            {
                var tipGO = pair.Value.gameObject;
                tipGO.SetActive(pair.Key == tipSide);
            }

            var initialPosition = _tips[tipSide].localPosition;
            Vector3 messagePanelPoint;
            
            if (_side == MessagePanelSide.Top || _side == MessagePanelSide.Bottom)
            {
                var offsetFromLeft = fullWidth * _messageTextRect.pivot.x;
                messagePanelPoint = _corners[0] + Vector3.right * offsetFromLeft;
            }
            else
            {
                var offsetFromBottom = fullHeight * _messageTextRect.pivot.y;
                messagePanelPoint = _corners[0] + Vector3.up * offsetFromBottom;
            }

            var localPoint = _messageTextRect.InverseTransformPoint(messagePanelPoint);

            if (_side == MessagePanelSide.Top || _side == MessagePanelSide.Bottom)
                localPoint.y = initialPosition.y;
            else
                localPoint.x = initialPosition.x;

            _tips[tipSide].localPosition = localPoint;
        }

        private void UpdatePivot(float fullWidth, float fullHeight)
        {
            _messageTextRect.GetWorldCorners(_corners);
            Vector2 newPivot;
            
            if (_side == MessagePanelSide.Top || _side == MessagePanelSide.Bottom)
            {
                var xDistance = Model.MessageOrigin.x - _corners[BottomLeftCornerIndex].x;
                newPivot = new Vector2(xDistance / fullWidth, _side == MessagePanelSide.Top ? 0 : 1);
            }
            else
            {
                var yDistance = Model.MessageOrigin.y - _corners[BottomLeftCornerIndex].y;
                newPivot = new Vector2(_side == MessagePanelSide.Right ? 0 : 1, yDistance / fullHeight);
            }

            var size = new Vector2(fullWidth, fullHeight);
            var deltaPivot = _messageTextRect.pivot - newPivot;
            var offset = new Vector2(deltaPivot.x * size.x, deltaPivot.y * size.y);

            offset /= _canvasScaleFactor;
            
            _messageTextRect.pivot = newPivot;
            _messageTextRect.localPosition -= (Vector3)offset;
        }
        
        private MessagePanelSide GetTipSideFromMessageSide(MessagePanelSide side)
        {
            switch (side)
            {
                case MessagePanelSide.Top:
                    return MessagePanelSide.Bottom;
                case MessagePanelSide.Right:
                    return MessagePanelSide.Left;
                case MessagePanelSide.Bottom:
                    return MessagePanelSide.Top;
                case MessagePanelSide.Left:
                    return MessagePanelSide.Right;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}