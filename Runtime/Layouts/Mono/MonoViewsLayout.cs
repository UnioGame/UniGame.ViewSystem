namespace UniGame.UiSystem.Runtime.WindowStackControllers
{
    using UniModules.UniGame.Core.Runtime.Attributes;
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using Backgrounds.Abstract;
    using Core.Runtime;
    using ViewSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.WindowStackControllers.Abstract;
    using UniRx;
    using UnityEngine.Serialization;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    public class MonoViewsLayout : MonoBehaviour, IViewLayout
    {
        #region inspector

        [SerializeField]
        public Canvas layoutCanvas;

        [SerializeField]
        public BackgroundFactory backgroundFactory;

        [SerializeReference]
#if ODIN_INSPECTOR
        [Required]
        [InlineEditor]
        [TitleGroup("Layout Behaviour")]
#endif
        [AssetFilter(typeof(ViewLayoutType))]
        public ViewLayoutType layoutBehaviour;

        #endregion

        private Lazy<IViewLayout> _viewLayout;
        
        private Lazy<IViewLayout> ViewLayout => _viewLayout = _viewLayout ?? new Lazy<IViewLayout>(Create);

        public IViewLayout LayoutController => ViewLayout.Value;

        public Transform Layout => LayoutController.Layout;
        
        public IObservable<IView> OnHidden => LayoutController.OnHidden;
        
        public IObservable<IView> OnShown => LayoutController.OnShown;
        
        public IObservable<IView> OnClosed => LayoutController.OnClosed;
        
        public IObservable<Type>  OnIntent => LayoutController.OnIntent;

        public IObservable<IView> OnBeginHide  => LayoutController.OnBeginHide;
        
        public IObservable<IView> OnBeginShow  => LayoutController.OnBeginShow;
        
        public ILifeTime LifeTime => LayoutController.LifeTime;
        
        #region public methods

        public void Dispose() => LayoutController.Dispose();

        public bool Contains(IView view) => LayoutController.Contains(view);

        public void HideAll() => LayoutController.HideAll();

        public IReadOnlyReactiveProperty<IView> ActiveView => LayoutController.ActiveView;

        public LayoutIntentResult Intent(string viewKey) => LayoutController.Intent(viewKey);

        public void Push(IView view) => LayoutController.Push(view);

        public TView Get<TView>() where TView : class,IView
        {
            return LayoutController.Get<TView>();
        }

        public List<TView> GetAll<TView>() where TView : class, IView
        {
            return LayoutController.GetAll<TView>();
        }

        public void CloseAll() => LayoutController.CloseAll();

        public void ShowLast() => LayoutController.ShowLast();

        public void Suspend()
        {
            if (layoutCanvas != null)
                layoutCanvas.enabled = false;
        }

        public void Resume()
        {
            if (layoutCanvas != null)
                layoutCanvas.enabled = true;
        }

        #endregion

        #region private methods

        protected virtual IViewLayout Create()
        {
            IBackgroundView backgroundView = null;
            
            if (backgroundFactory != null) {
                backgroundView = backgroundFactory
                    .Create(layoutCanvas?.transform);
                backgroundView.Hide();
            }
            
            if(layoutBehaviour == null)
                throw new NullReferenceException(nameof(layoutBehaviour));

            var layoutAsset = Instantiate(layoutBehaviour);
            
            return layoutAsset.Create(layoutCanvas.transform, backgroundView);
        }

        protected void OnDestroy()
        {
            if(ViewLayout.IsValueCreated)
                ViewLayout.Value.Dispose();
        }

        #endregion
    }
}