namespace UniGame.UiSystem.Runtime.ViewsFlow
{
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Flow/SingleScreenFlow",fileName = nameof(SingleActiveScreenFlow))]
    public class SingleActiveScreenFlowAsset : ViewFlowControllerAsset
    {
        public override IViewFlowController Create()
        {
            return new SingleActiveScreenFlow();
        }
    }
}
