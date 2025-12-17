namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Shows a preview of a texture, sprite, or model reference
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PreviewAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Height of the preview in pixels
        /// </summary>
        public int Height { get; set; } = 100;

        /// <summary>
        /// Width of the preview in pixels
        /// </summary>
        public int Width { get; set; } = 100;

        public PreviewAttribute()
        {
        }

        public PreviewAttribute(int height)
        {
            Height = height;
            Width = height;
        }

        public PreviewAttribute(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
