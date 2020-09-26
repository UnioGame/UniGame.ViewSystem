namespace UniGame.UiSystem.Runtime.ViewsFlow
{
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
