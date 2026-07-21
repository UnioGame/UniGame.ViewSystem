namespace UniGame.ViewSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using UniGame.Core.Runtime.ObjectPool;
    using UnityEngine;
    using UnityEngine.UI;

    [Flags]
    public enum ViewTransformStateFlags
    {
        None = 0,
        AnchoredPosition = 1 << 0,
        SizeDelta = 1 << 1,
        Anchors = 1 << 2,
        Pivot = 1 << 3,
        LocalRotation = 1 << 4,
        LocalScale = 1 << 5,
        All = AnchoredPosition | SizeDelta | Anchors | Pivot | LocalRotation | LocalScale,
    }

    public sealed class ViewStateSnapshot : IPoolable
    {
        public readonly Dictionary<int, TransformState> Transforms = new(8);
        public readonly Dictionary<int, ColorState> Colors = new(4);
        public readonly Dictionary<int, ActiveState> ActiveStates = new(4);

        public ViewStateSnapshot Cache(RectTransform target, ViewTransformStateFlags flags)
        {
            if (!target)
                return this;

            var instanceId = target.GetInstanceID();
            if (Transforms.TryGetValue(instanceId, out var state))
            {
                state.Flags |= flags;
                Transforms[instanceId] = state;
                return this;
            }

            Transforms.Add(instanceId, new TransformState
            {
                Target = target,
                Flags = flags,
                AnchoredPosition = target.anchoredPosition3D,
                SizeDelta = target.sizeDelta,
                AnchorMin = target.anchorMin,
                AnchorMax = target.anchorMax,
                Pivot = target.pivot,
                LocalRotation = target.localRotation,
                LocalScale = target.localScale,
            });

            return this;
        }

        public ViewStateSnapshot Cache(
            IReadOnlyList<RectTransform> targets,
            ViewTransformStateFlags flags)
        {
            for (var i = 0; i < targets.Count; i++)
                Cache(targets[i], flags);

            return this;
        }

        public ViewStateSnapshot CacheColor(Graphic target)
        {
            var instanceId = target.GetInstanceID();
            if (!Colors.ContainsKey(instanceId))
                Colors.Add(instanceId, new ColorState(target, target.color));

            return this;
        }

        public ViewStateSnapshot CacheActive(GameObject target)
        {
            var instanceId = target.GetInstanceID();
            if (!ActiveStates.ContainsKey(instanceId))
                ActiveStates.Add(instanceId, new ActiveState(target, target.activeSelf));

            return this;
        }

        public ViewStateSnapshot Restore()
        {
            foreach (var state in Transforms.Values)
                state.Restore();

            foreach (var state in Colors.Values)
                state.Target.color = state.Color;

            foreach (var state in ActiveStates.Values)
                state.Target.SetActive(state.IsActive);

            return this;
        }

        public void Release()
        {
            Transforms.Clear();
            Colors.Clear();
            ActiveStates.Clear();
        }

        public struct TransformState
        {
            public RectTransform Target;
            public ViewTransformStateFlags Flags;
            public Vector3 AnchoredPosition;
            public Vector2 SizeDelta;
            public Vector2 AnchorMin;
            public Vector2 AnchorMax;
            public Vector2 Pivot;
            public Quaternion LocalRotation;
            public Vector3 LocalScale;

            public void Restore()
            {
                if ((Flags & ViewTransformStateFlags.AnchoredPosition) != 0)
                    Target.anchoredPosition3D = AnchoredPosition;
                if ((Flags & ViewTransformStateFlags.SizeDelta) != 0)
                    Target.sizeDelta = SizeDelta;
                if ((Flags & ViewTransformStateFlags.Anchors) != 0)
                {
                    Target.anchorMin = AnchorMin;
                    Target.anchorMax = AnchorMax;
                }
                if ((Flags & ViewTransformStateFlags.Pivot) != 0)
                    Target.pivot = Pivot;
                if ((Flags & ViewTransformStateFlags.LocalRotation) != 0)
                    Target.localRotation = LocalRotation;
                if ((Flags & ViewTransformStateFlags.LocalScale) != 0)
                    Target.localScale = LocalScale;
            }
        }

        public readonly struct ColorState
        {
            public ColorState(Graphic target, Color color)
            {
                Target = target;
                Color = color;
            }

            public Graphic Target { get; }
            public Color Color { get; }
        }

        public readonly struct ActiveState
        {
            public ActiveState(GameObject target, bool isActive)
            {
                Target = target;
                IsActive = isActive;
            }

            public GameObject Target { get; }
            public bool IsActive { get; }
        }
    }
}
