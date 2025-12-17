namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Adds space before a field in the inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PropertySpaceAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Space before the field in pixels
        /// </summary>
        public float SpaceBefore { get; }

        /// <summary>
        /// Space after the field in pixels
        /// </summary>
        public float SpaceAfter { get; set; } = 0;

        public PropertySpaceAttribute(float spaceBefore = 10)
        {
            SpaceBefore = spaceBefore;
        }
    }
}
