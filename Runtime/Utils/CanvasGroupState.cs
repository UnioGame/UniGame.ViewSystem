namespace UniGreenModules.UniUiSystem.Runtime.Utils
{
    using System;

    [Serializable]
    public struct CanvasGroupState
    {
        public float Alpha;
        public bool  Interactable;
        public bool  BlockRaycasts;
        public bool  IgnoreParent;
    }
}