namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Shows a full asset preview for the assigned object
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AssetPreviewAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Whether to allow selecting a different asset
        /// </summary>
        public bool AllowSceneObjects { get; set; } = false;

        public AssetPreviewAttribute()
        {
        }

        public AssetPreviewAttribute(bool allowSceneObjects)
        {
            AllowSceneObjects = allowSceneObjects;
        }
    }
}
