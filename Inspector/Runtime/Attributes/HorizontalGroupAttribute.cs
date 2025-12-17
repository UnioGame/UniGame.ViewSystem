namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Groups fields horizontally (in a row) in the inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class HorizontalGroupAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// The group identifier
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Width of the field (flexible or fixed size)
        /// </summary>
        public float Width { get; set; } = -1; // -1 means flexible

        public HorizontalGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
