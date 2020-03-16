namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UnityEngine;

    public class WindowView<TWindowModel> : 
        UiGroupView<TWindowModel> where TWindowModel : class, IViewModel
    {

    }
}