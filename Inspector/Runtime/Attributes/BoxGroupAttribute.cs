namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Groups fields in a visual box container in the inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BoxGroupAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// The group name/title
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Show/hide toggle for the group
        /// </summary>
        public bool ShowToggle { get; set; } = true;

        /// <summary>
        /// Whether the group starts in expanded state
        /// </summary>
        public bool Expanded { get; set; } = true;

        public BoxGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
