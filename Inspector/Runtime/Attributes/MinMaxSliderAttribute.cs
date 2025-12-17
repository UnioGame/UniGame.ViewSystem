namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Displays a min/max slider for Vector2 fields (min is X, max is Y)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MinMaxSliderAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Minimum value for the slider
        /// </summary>
        public float Min { get; }

        /// <summary>
        /// Maximum value for the slider
        /// </summary>
        public float Max { get; }

        public MinMaxSliderAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
