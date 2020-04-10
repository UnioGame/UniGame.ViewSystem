namespace UniGame.UiSystem.Runtime.ViewsFlow
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/UiSystem/Flow/SingleScreenFlow",fileName = nameof(SingleActiveScreenFlow))]
    public class SingleActiveScreenFlowAsset : ViewFlowControllerAsset
    {
        public override IViewFlowController Create()
        {
            return new SingleActiveScreenFlow();
        }
    }
}
