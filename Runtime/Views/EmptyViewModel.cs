using System;
using UniGame.UiSystem.Runtime;

[Serializable]
public class EmptyViewModel : ViewModelBase
{
    public sealed override bool IsDisposeWithModel => false;
    
}
