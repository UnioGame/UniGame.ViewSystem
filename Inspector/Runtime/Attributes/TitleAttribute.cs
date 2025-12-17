namespace UniGame.ViewSystem.Inspector
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Adds a title/header above a field in the inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TitleAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// The title text to display
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Optional subtitle text
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Title text color (as hex or standard color names)
        /// </summary>
        public string Color { get; set; } = "white";

        public TitleAttribute(string title)
        {
            Title = title;
        }

        /// <summary>
        /// Constructor with title and subtitle
        /// </summary>
        public TitleAttribute(string title, string subtitle)
        {
            Title = title;
            Subtitle = subtitle;
        }
    }
}
