namespace Game.Modules.UnioModules.UniGame.ViewSystem.Runtime.Animations
{
    using System;
    using Cysharp.Threading.Tasks;
    using global::UniGame.Core.Runtime;
    using global::UniGame.ViewSystem.Runtime;
    using global::UniModules.UniUiSystem.Runtime.Utils;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.Serialization;
    using Views.Abstract;

    [Serializable]
    public class SimpleFadeViewAnimation : IViewAnimation
    {
        #region inspector

        [TitleGroup("Animation Settings")] public bool enabled = false;

        [ShowIf(nameof(enabled))] [TitleGroup("Animation Settings")]
        public bool animateShowing = true;

        [ShowIf(nameof(enabled))] [TitleGroup("Animation Settings")]
        public bool animateHiding = true;

        [ShowIf(nameof(enabled))] [TitleGroup("Animation Settings")]
        public bool animateClosing = true;

        [ShowIf(nameof(enabled))]
        [TitleGroup("Animation Settings")]
        public float duration = 0.2f;

        [ShowIf(nameof(enabled))] [FoldoutGroup("CanvasGroup")] [SerializeField]
        public CanvasGroupState hiddenState = new CanvasGroupState
        {
            Alpha = 0,
            BlockRaycasts = false,
            Interactable = false
        };

        [ShowIf(nameof(enabled))] [FoldoutGroup("CanvasGroup")] [SerializeField]
        public CanvasGroupState visibleState = new CanvasGroupState
        {
            Alpha = 1,
            BlockRaycasts = true,
            Interactable = true
        };

        #endregion

        public async UniTask Show(IView view, ILifeTime lifeTime)
        {
            if (!animateShowing)
                return;

            if (!enabled)
                return;

            await AnimateFade(view, visibleState, hiddenState, duration, lifeTime);
        }

        public async UniTask Close(IView view, ILifeTime lifeTime)
        {
            if (!animateClosing) return;
            await HideAnimation(view, lifeTime);
        }

        public async UniTask Hide(IView view, ILifeTime lifeTime)
        {
            if (!animateHiding) return;
            await HideAnimation(view, lifeTime);
        }

        public async UniTask HideAnimation(IView view, ILifeTime lifeTime)
        {
            if (!enabled)
                return;

            await AnimateFade(view, visibleState, hiddenState, duration, lifeTime);
        }

        public CanvasGroup GetGroup(IView view)
        {
            var gameObject = view.GameObject;

            var canvasGroup = gameObject.GetComponent<CanvasGroup>();
            canvasGroup ??= gameObject.AddComponent<CanvasGroup>();

            return canvasGroup;
        }

        private async UniTask AnimateFade(IView view, CanvasGroupState from, CanvasGroupState to, float duration,
            ILifeTime progressLifeTime)
        {
            var canvasGroup = GetGroup(view);
            if (canvasGroup == null) return;

            if (duration <= 0)
            {
                canvasGroup.SetState(to);
                return;
            }

            canvasGroup.SetState(from);

            var fromAlpha = from.Alpha;
            var toAlpha = to.Alpha;

            var token = progressLifeTime.CancellationToken;
            var startTime = Time.time;
            var finishTime = Time.time + duration;

            try
            {
                while (!token.IsCancellationRequested && Time.time < finishTime)
                {
                    var timePassed = (Time.time - startTime);
                    var progress = timePassed / duration;
                    var alpha = Mathf.Lerp(fromAlpha, toAlpha, progress);
                    canvasGroup.alpha = alpha;
                    await UniTask.Yield();
                }
            }
            finally
            {
                canvasGroup.SetState(to);
            }
        }
    }
}