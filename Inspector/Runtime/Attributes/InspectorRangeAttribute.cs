namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Restricts a float or int field to a specified range with a slider
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InspectorRangeAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// The minimum value of the range
        /// </summary>
        public float Min { get; }

        /// <summary>
        /// The maximum value of the range
        /// </summary>
        public float Max { get; }

        public InspectorRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
