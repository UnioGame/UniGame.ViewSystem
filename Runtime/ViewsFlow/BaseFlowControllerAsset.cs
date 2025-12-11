namespace UniGame.UiSystem.Runtime.ViewsFlow
{
    using UnityEngine;
    using ViewSystem.Runtime;

    public abstract class BaseFlowControllerAsset : ScriptableObject
    {
        public abstract IViewFlowController Create();
    }
}