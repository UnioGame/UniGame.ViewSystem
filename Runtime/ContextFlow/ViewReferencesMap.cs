namespace UniGame.ViewSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using UniGame.UiSystem.Runtime.Settings;
    using UniGame.Runtime.Utils;
    using UniGame.DataStructure;
    using UnityEngine;

    [Serializable]
    public class ViewReferencesMap : IViewReferencesMap
    {
        #region static data
        
        public static readonly UiViewReference Empty = new()
        {
            ViewName = "EMPTY",
            ModelType = typeof(EmptyViewModel),
            ViewModelType = typeof(EmptyViewModel),
        };
        
        private static readonly UiReferenceList EmptyList = new();
        
        private static readonly MemorizeItem<string,string> KeyCache = 
            MemorizeTool.Memorize<string,string>(static x => x.ToLower());

        #endregion
        
        public UiReferenceDictionary references = new(16);

        public IReadOnlyList<UiViewReference> this[string view] => Find(view);
        
        public IReadOnlyList<UiViewReference> Find(string view)
        {
            var key = KeyCache[view];
            
            var result = references.TryGetValue(key, out var items) 
                ? items.references 
                : EmptyList.references;
            
            return result;
        }

        public void Add(UiViewReference reference)
        {
            if (reference == null)
                return;

            var type = (Type)reference.Type;
            var modelType = (Type)reference.ModelType;
            var viewModelType = (Type)reference.ViewModelType;

            if (type == null || modelType == null || viewModelType == null)
            {
                Debug.LogWarning($"ViewSystem: skip invalid UiViewReference '{reference.ViewName ?? reference.AssetGUID}' because serialized view/model type is missing. View={type?.Name ?? "null"} Model={modelType?.Name ?? "null"} ViewModel={viewModelType?.Name ?? "null"}");
                return;
            }

            var viewName = string.IsNullOrWhiteSpace(reference.ViewName)
                ? type.Name
                : reference.ViewName;

            var targetName = KeyCache[viewName];
            var targetType = KeyCache[type.Name];
            var targetModelName = KeyCache[modelType.Name];
            var targetViewModelName = KeyCache[viewModelType.Name];
            
            AddReference(targetName,reference);
            AddReference(targetType,reference);
            AddReference(targetModelName,reference);
            AddReference(targetViewModelName,reference);
        }
        
        private void AddReference(string name,UiViewReference reference)
        {
            if (!references.TryGetValue(name, out var items) )
            {
                items = new UiReferenceList();
                references[name] = items;
            }

            if (items.references.Contains(reference)) return;
            
            items.references.Add(reference);
        }
        

    }

    [Serializable]
    public class UiReferenceDictionary : SerializableDictionary<string,UiReferenceList>
    {
        
        public UiReferenceDictionary(int capacity) : base(capacity)
        {
        }
        
    }

}