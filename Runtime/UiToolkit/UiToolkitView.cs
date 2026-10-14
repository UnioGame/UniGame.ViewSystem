namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using Cysharp.Threading.Tasks;
    using UniGame.ViewSystem.Runtime;
    using UniGame.ViewSystem.Runtime.Views.Abstract;

    /// <summary>
    /// Optional ViewSystem base for screen UI Toolkit views. uGUI views remain
    /// unchanged and an instance still exposes exactly one IView component.
    /// </summary>
    public abstract class UiToolkitView<TViewModel> : View<TViewModel>
        where TViewModel : class, IViewModel
    {
        private readonly UiToolkitViewAnimation _toolkitAnimation = new();
        private UiToolkitViewPresentation _presentation;

        public UiToolkitViewPresentation Presentation =>
            _presentation != null ? _presentation : _presentation = GetComponent<UiToolkitViewPresentation>();

        public bool IsToolkitReady => Presentation != null && Presentation.IsReady;

        protected override IViewAnimation SelectAnimation() => _toolkitAnimation;

        protected override async UniTask OnInitialize(TViewModel model)
        {
            var presentation = Presentation;
            if (presentation == null)
                throw new System.InvalidOperationException($"UITK view '{ViewId}' on '{name}' requires {nameof(UiToolkitViewPresentation)}.");

            LifeTime.AddCleanUpAction(presentation.ClearBindings);
            await base.OnInitialize(model);

            if (gameObject.activeInHierarchy && presentation.Document != null && presentation.Document.isActiveAndEnabled)
            {
                presentation.Initialize(ViewId);
                await OnInitializeToolkit(model, presentation.Root);
            }
            else
            {
                presentation.ClearBindings();
            }
        }

        /// <summary>Bind the current model to the current UIDocument tree.</summary>
        protected virtual UniTask OnInitializeToolkit(TViewModel model, UnityEngine.UIElements.VisualElement root) => UniTask.CompletedTask;

        protected override void OnViewEnable()
        {
            base.OnViewEnable();
            var presentation = Presentation;
            if (presentation == null) return;

            if (Model == null)
            {
                presentation.RefreshTree(ViewId);
                return;
            }

            if (presentation.IsReady)
                presentation.RefreshTree(ViewId);
            else
            {
                presentation.Initialize(ViewId);
                presentation.SetVisible(Status.CurrentValue is ViewStatus.Showing or ViewStatus.Shown);
            }

            RebindToolkit(Model, presentation).Forget();
        }

        protected override void OnViewDisable()
        {
            Presentation?.Suspend();
            base.OnViewDisable();
        }

        protected override void OnViewStatus(ViewStatus status)
        {
            base.OnViewStatus(status);
            Presentation?.SetVisible(status == ViewStatus.Showing || status == ViewStatus.Shown);
        }

        private async UniTask RebindToolkit(TViewModel model, UiToolkitViewPresentation presentation)
        {
            await OnInitializeToolkit(model, presentation.Root);
        }
    }
}
