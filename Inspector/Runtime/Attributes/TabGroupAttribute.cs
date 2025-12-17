namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Groups fields into tabs within the inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TabGroupAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// The tab group name
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// The tab name within the group
        /// </summary>
        public string TabName { get; }

        public TabGroupAttribute(string groupName, string tabName = "Default")
        {
            GroupName = groupName;
            TabName = tabName;
        }
    }
}
