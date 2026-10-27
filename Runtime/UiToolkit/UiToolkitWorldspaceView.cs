namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using UniGame.ViewSystem.Runtime;
    using UnityEngine;

    /// <summary>Faces a world-space UI Toolkit view toward the active game camera.</summary>
    public abstract class UiToolkitWorldspaceView<TViewModel> : UiToolkitView<TViewModel>
        where TViewModel : class, IViewModel
    {
        protected virtual void LateUpdate()
        {
            var camera = Camera.main;
            if (camera != null)
                transform.rotation = camera.transform.rotation;
        }
    }
}
