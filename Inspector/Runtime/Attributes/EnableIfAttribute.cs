namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Enables or disables a field based on a condition
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EnableIfAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Name of the field/property to check
        /// </summary>
        public string ConditionField { get; }

        /// <summary>
        /// Expected value for the condition to be true
        /// </summary>
        public object ExpectedValue { get; set; }

        public EnableIfAttribute(string conditionField)
        {
            ConditionField = conditionField;
            ExpectedValue = true;
        }

        public EnableIfAttribute(string conditionField, object expectedValue)
        {
            ConditionField = conditionField;
            ExpectedValue = expectedValue;
        }
    }
}
