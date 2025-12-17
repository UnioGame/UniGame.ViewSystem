namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Disables a field based on a condition
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DisableIfAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Name of the field/property to check
        /// </summary>
        public string ConditionField { get; }

        /// <summary>
        /// Expected value for the condition to disable the field
        /// </summary>
        public object ExpectedValue { get; set; }

        public DisableIfAttribute(string conditionField)
        {
            ConditionField = conditionField;
            ExpectedValue = true;
        }

        public DisableIfAttribute(string conditionField, object expectedValue)
        {
            ConditionField = conditionField;
            ExpectedValue = expectedValue;
        }
    }
}
