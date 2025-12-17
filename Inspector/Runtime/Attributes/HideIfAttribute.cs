namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Hides a field conditionally based on another field's value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class HideIfAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Name of the boolean field or method to check
        /// </summary>
        public string ConditionName { get; }

        /// <summary>
        /// Whether to invert the condition
        /// </summary>
        public bool Invert { get; set; } = false;

        public HideIfAttribute(string conditionName)
        {
            ConditionName = conditionName;
        }
    }
}
