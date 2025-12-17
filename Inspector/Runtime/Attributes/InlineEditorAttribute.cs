namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Displays a reference as an inline editor instead of a selectable object
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InlineEditorAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Whether to draw the toolbar buttons (eye, drag, etc.)
        /// </summary>
        public bool DrawToolbar { get; set; } = true;

        /// <summary>
        /// Whether to draw the header for the object
        /// </summary>
        public bool DrawHeader { get; set; } = true;

        public InlineEditorAttribute()
        {
        }

        public InlineEditorAttribute(bool drawToolbar, bool drawHeader)
        {
            DrawToolbar = drawToolbar;
            DrawHeader = drawHeader;
        }
    }
}
