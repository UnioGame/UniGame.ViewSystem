namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Shows a field conditionally based on another field's value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ShowIfAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Name of the boolean field or method to check
        /// </summary>
        public string ConditionName { get; }

        /// <summary>
        /// Whether to invert the condition (show if false instead of true)
        /// </summary>
        public bool Invert { get; set; } = false;

        public ShowIfAttribute(string conditionName)
        {
            ConditionName = conditionName;
        }
    }
}
