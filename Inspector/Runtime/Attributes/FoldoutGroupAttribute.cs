namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Groups fields in a collapsible foldout section
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FoldoutGroupAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// The group name/title
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Whether the foldout is expanded by default
        /// </summary>
        public bool ExpandedByDefault { get; set; } = true;

        public FoldoutGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }

        public FoldoutGroupAttribute(string groupName, bool expandedByDefault)
        {
            GroupName = groupName;
            ExpandedByDefault = expandedByDefault;
        }
    }
}
