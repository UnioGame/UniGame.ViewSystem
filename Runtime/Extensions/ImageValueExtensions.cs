namespace UniGame.ViewSystem.Runtime.Extensions.Images
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>Exposes dependency-focused value updates for images.</summary>
    public static class ImageValueExtensions
    {
        /// <summary>Updates an image color.</summary>
        public static bool SetValue(this Image target, Color value)
        {
            return ValuesExtensions.SetValue(target, value);
        }
    }
}
