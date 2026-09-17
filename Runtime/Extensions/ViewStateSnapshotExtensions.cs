namespace UniGame.ViewSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using UniGame.Core.Runtime;
    using UniGame.Runtime.ObjectPool;
    using UnityEngine;
    using Object = UnityEngine.Object;
#if UNITY_6000_3_OR_NEWER
    using ObjectId = UnityEngine.EntityId;
#else
    using ObjectId = System.Int32;
#endif

    public static class ViewStateSnapshotExtensions
    {
        private const int DefaultCapacity = 32;

        private static readonly Dictionary<ObjectId, RegistryEntry> Registry = new(DefaultCapacity);

        public static ViewStateSnapshot CacheViewState(this Object owner)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            var instanceId = GetObjectId(owner);
            if (Registry.TryGetValue(instanceId, out var entry))
            {
                if (ReferenceEquals(entry.Owner, owner))
                    return entry.Snapshot;

                Release(instanceId);
            }

            var snapshot = ClassPool.Spawn<ViewStateSnapshot>();
            Registry.Add(instanceId, new RegistryEntry(owner, snapshot));
            AssetLifeTime.GetAssetLifeTime(owner)
                .AddCleanUpAction(instanceId, static id => Release(id));

            return snapshot;
        }

        public static ViewStateSnapshot CacheViewState(
            this Object owner,
            RectTransform target,
            ViewTransformStateFlags flags)
        {
            return owner.CacheViewState().Cache(target, flags);
        }

        public static bool TryGetViewState(this Object owner, out ViewStateSnapshot snapshot)
        {
            if (owner != null &&
                Registry.TryGetValue(GetObjectId(owner), out var entry) &&
                ReferenceEquals(entry.Owner, owner))
            {
                snapshot = entry.Snapshot;
                return true;
            }

            snapshot = null;
            return false;
        }

        public static bool RestoreViewState(this Object owner)
        {
            if (!owner.TryGetViewState(out var snapshot))
                return false;

            snapshot.Restore();
            return true;
        }

        public static bool RestoreViewState(this Object owner, ViewStateSnapshot snapshot)
        {
            if (!owner.TryGetViewState(out var registeredSnapshot) ||
                !ReferenceEquals(registeredSnapshot, snapshot))
                return false;

            snapshot.Restore();
            return true;
        }

        public static bool ReleaseViewState(this Object owner)
        {
            if (owner == null)
                return false;

            var instanceId = GetObjectId(owner);
            if (!Registry.TryGetValue(instanceId, out var entry) ||
                !ReferenceEquals(entry.Owner, owner))
                return false;

            Release(instanceId);
            return true;
        }

        private static ObjectId GetObjectId(Object owner)
        {
#if UNITY_6000_3_OR_NEWER
            return owner.GetEntityId();
#else
            return owner.GetInstanceID();
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            foreach (var entry in Registry.Values)
                ClassPool.Despawn(entry.Snapshot);

            Registry.Clear();
        }

        private static void Release(ObjectId instanceId)
        {
            if (!Registry.Remove(instanceId, out var entry))
                return;

            ClassPool.Despawn(entry.Snapshot);
        }

        private readonly struct RegistryEntry
        {
            public RegistryEntry(Object owner, ViewStateSnapshot snapshot)
            {
                Owner = owner;
                Snapshot = snapshot;
            }

            public Object Owner { get; }
            public ViewStateSnapshot Snapshot { get; }
        }
    }
}
