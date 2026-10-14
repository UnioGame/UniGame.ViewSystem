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

            if (gameObject.activeInHierarchy && presentation.HasActiveHost)
            {
                presentation.Initialize(ViewId);
                presentation.RootChanged -= OnRootChanged;
                presentation.RootChanged += OnRootChanged;
                if (presentation.IsReady)
                    await BindToolkit(model, presentation, presentation.Root);
                else
                    await UniTask.WaitUntil(() => presentation.IsReady)
                        .AttachExternalCancellation(LifeTime.Token);
            }
            else
            {
                presentation.ClearBindings();
            }
        }

        /// <summary>Bind the current model to the current UI Toolkit tree.</summary>
        protected virtual UniTask OnInitializeToolkit(TViewModel model, UnityEngine.UIElements.VisualElement root) => UniTask.CompletedTask;

        protected override void OnViewEnable()
        {
            base.OnViewEnable();
            var presentation = Presentation;
            if (presentation == null) return;
            presentation.RootChanged -= OnRootChanged;
            presentation.RootChanged += OnRootChanged;

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

            if (presentation.IsReady)
                RebindToolkit(Model, presentation, presentation.Root).Forget();
        }

        protected override void OnViewDisable()
        {
            var presentation = Presentation;
            if (presentation != null)
            {
                presentation.RootChanged -= OnRootChanged;
                presentation.Suspend();
            }
            base.OnViewDisable();
        }

        protected override void OnViewStatus(ViewStatus status)
        {
            base.OnViewStatus(status);
            Presentation?.SetVisible(status == ViewStatus.Showing || status == ViewStatus.Shown);
        }

        private void OnRootChanged(UnityEngine.UIElements.VisualElement root)
        {
            if (Model != null && isActiveAndEnabled && root != null)
                RebindToolkit(Model, Presentation, root).Forget();
        }

        private async UniTask BindToolkit(
            TViewModel model,
            UiToolkitViewPresentation presentation,
            UnityEngine.UIElements.VisualElement root)
        {
            await OnInitializeToolkit(model, root);
        }

        private async UniTask RebindToolkit(
            TViewModel model,
            UiToolkitViewPresentation presentation,
            UnityEngine.UIElements.VisualElement root)
        {
            presentation.ClearBindings();
            await BindToolkit(model, presentation, root);
        }
    }
}
