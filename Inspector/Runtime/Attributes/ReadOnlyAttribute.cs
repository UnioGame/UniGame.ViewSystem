namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Makes a field read-only in the inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Whether to show the field in gray (disabled) style
        /// </summary>
        public bool ShowDisabled { get; set; } = true;
    }
}
