namespace UniGame.UiSystem.Runtime.ViewsFlow
{
    using ViewSystem.Runtime;
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
