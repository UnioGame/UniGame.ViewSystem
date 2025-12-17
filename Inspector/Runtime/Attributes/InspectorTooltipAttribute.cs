namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Adds a tooltip to a field in the inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InspectorTooltipAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// The tooltip text
        /// </summary>
        public string Text { get; }

        public InspectorTooltipAttribute(string text)
        {
            Text = text;
        }
    }
}
