namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Displays a button in the inspector that calls a parameterless method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ButtonAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Display name for the button (if null, uses method name)
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Button height in pixels
        /// </summary>
        public float Height { get; set; } = 30;

        public ButtonAttribute(string label = null)
        {
            Label = label;
        }
    }
}
