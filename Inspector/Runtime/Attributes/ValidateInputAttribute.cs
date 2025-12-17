namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Validates the input of a field using a method
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidateInputAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Name of the validation method
        /// </summary>
        public string ValidationMethod { get; }

        /// <summary>
        /// Message to display when validation fails
        /// </summary>
        public string ErrorMessage { get; set; } = "Validation failed";

        public ValidateInputAttribute(string validationMethod)
        {
            ValidationMethod = validationMethod;
        }

        public ValidateInputAttribute(string validationMethod, string errorMessage)
        {
            ValidationMethod = validationMethod;
            ErrorMessage = errorMessage;
        }
    }
}
