namespace UniGame.UI.Common.Views
{
    using System.Collections.Generic;
    using Context.Runtime;
    using Cysharp.Threading.Tasks;
    using global::Extensions;
    using UniGame.Core.Runtime;
    using UniGame.Core.Runtime.SerializableType;
    using UniGame.UiSystem.Runtime.Settings;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniGame.UiSystem.Runtime;
    using UnityEngine;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    public class ViewInstancer : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [ValueDropdown(nameof(GetViewsDropdown))]
        [InlineButton(nameof(PingView))]
#endif
        public SType viewType;

        public ViewType layout = ViewType.None;
        public string viewTag = string.Empty;
        public string viewName = string.Empty;
        public bool stayWorldPosition = false;

        public RectTransform container;
        
        public bool createOnStart = true;

        public GameObject viewInstance;
        
#if ODIN_INSPECTOR
        public IEnumerable<ValueDropdownItem<SType>> GetViewsDropdown()
        {
            foreach (var viewType in UiViewReference.GetTypeDropdown(typeof(IView)))
            {
                yield return viewType;
            }
        }
#endif

        
        public async UniTask CreateViewAsync()
        {
            var type = viewType.Type;
            if (viewInstance || type == null) return;

            var context = await GameContext.GetContextAsync()
                .AttachExternalCancellation<IContext>(destroyCancellationToken);
            
            var viewSystem = await context
                .ReceiveFirstAsync<IGameViewSystem>()
                .AttachExternalCancellation<IGameViewSystem>(destroyCancellationToken);

            IView view = null;
            var lifeTime = this.GetAssetLifeTime();

            switch (layout)
            {
                case ViewType.None:
                    var viewParent = container == null ? transform : container;
                    view = await viewSystem.Create(type,viewParent,viewTag,viewName,stayWorldPosition,lifeTime)
                        .AttachExternalCancellation<IView>(destroyCancellationToken);
                    view.GameObject.SetActive(true);
                    break;
                case ViewType.Screen:
                    view = await viewSystem.OpenScreen(type,viewTag,viewName)
                        .AttachExternalCancellation<IView>(destroyCancellationToken);
                    break;
                case ViewType.Window:
                    view = await viewSystem.OpenWindow(type,viewTag,viewName)
                        .AttachExternalCancellation<IView>(destroyCancellationToken);
                    break;
                case ViewType.Overlay:
                    view = await viewSystem.OpenOverlay(type,viewTag,viewName)
                        .AttachExternalCancellation<IView>(destroyCancellationToken);
                    break;
            }

            viewInstance = view?.GameObject;
        }

        public void PingView()
        {
            var type = viewType.Type;
            if (type == null) return;
            type.PingView();
        }
        
        private void Start()
        {
            if(createOnStart) 
                CreateViewAsync().Forget();
        }
    }
}