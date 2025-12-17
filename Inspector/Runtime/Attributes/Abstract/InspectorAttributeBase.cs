namespace UniGame.ViewSystem.Inspector
{
    using UnityEngine;
    
    /// <summary>
    /// Base class for all custom inspector attributes
    /// </summary>
    public abstract class InspectorAttributeBase : PropertyAttribute
    {
        /// <summary>
        /// Gets the display order of the attribute in the inspector
        /// </summary>
        public int Order { get; set; } = 0;
    }
}
