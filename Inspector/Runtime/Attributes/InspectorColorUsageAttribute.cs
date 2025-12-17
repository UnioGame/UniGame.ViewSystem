namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Configures how a color field is displayed in the inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InspectorColorUsageAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Whether to show the alpha channel
        /// </summary>
        public bool ShowAlpha { get; set; } = true;

        /// <summary>
        /// Whether to use HDR colors
        /// </summary>
        public bool UseHDR { get; set; } = false;

        public InspectorColorUsageAttribute()
        {
        }

        public InspectorColorUsageAttribute(bool showAlpha)
        {
            ShowAlpha = showAlpha;
        }

        public InspectorColorUsageAttribute(bool showAlpha, bool useHDR)
        {
            ShowAlpha = showAlpha;
            UseHDR = useHDR;
        }
    }
}
