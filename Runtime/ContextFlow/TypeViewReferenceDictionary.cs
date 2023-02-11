using System;
using System.Collections.Generic;
using UniGame.UiSystem.Runtime.Settings;
using UniModules.UniGame.Core.Runtime.DataStructure;
using UniGame.Core.Runtime.SerializableType;

namespace UniGame.ViewSystem.Runtime
{
    [Serializable]
    public class TypeViewReferenceDictionary : SerializableDictionary<SType,UiReferenceList >
    {
        private static readonly List<UiViewReference> _emptyList = new List<UiViewReference>(0);
        
        public TypeViewReferenceDictionary(int capacity) : base(capacity) { }
        
        public IReadOnlyList<UiViewReference> FindByType(Type type, bool strongMatching = true)
        {
            if (strongMatching)
            {
                return TryGetValue(type, out var items) ? 
                    items.references : _emptyList;
            }

            foreach (var view in this)
            {
                var viewType = view.Key;
                if (type.IsAssignableFrom(viewType))
                    return view.Value.references;
            }

            return _emptyList;
        }
        
    }
}