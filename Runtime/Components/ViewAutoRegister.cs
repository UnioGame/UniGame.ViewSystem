namespace Utils
{
    using Cysharp.Threading.Tasks;
    using UniGame.Context.Runtime;
    using UniGame.UiSystem.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UnityEngine;

    public class ViewAutoRegister : MonoBehaviour
    {
        public ViewBase target;
        public bool registerOnStart = true;
        public bool registerOnEnable = false;
        
        public async UniTask RegisterView()
        {
            var context = await GameContext.GetContextAsync()
                .AttachExternalCancellation(destroyCancellationToken);
            
            var viewSystem = await context
                .ReceiveFirstAsync<IGameViewSystem>()
                .AttachExternalCancellation(destroyCancellationToken);

            var targetView = target ?? gameObject.GetComponent<IView>();
            
            await targetView.RegisterView(destroyCancellationToken);
        }

        private void Start()
        {
            if (registerOnStart)
                RegisterView().Forget();
        }

        private void OnEnable()
        {
            if (registerOnEnable)
                RegisterView().Forget();
        }
    }
}