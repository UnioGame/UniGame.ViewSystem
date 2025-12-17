namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Sets the width of the field label
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LabelWidthAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Width of the label in pixels
        /// </summary>
        public float Width { get; }

        public LabelWidthAttribute(float width)
        {
            Width = width;
        }
    }
}
