namespace UniGame.UiSystem.Runtime.ViewsFlow
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/UiSystem/Flow/SindleView",fileName = nameof(SingleViewFlowAsset))]
    public class SingleViewFlowAsset : ViewFlowControllerAsset
    {
        public override IViewFlowController Create()
        {
            return new SingleActiveViewFlowController();
        }
    }
}
