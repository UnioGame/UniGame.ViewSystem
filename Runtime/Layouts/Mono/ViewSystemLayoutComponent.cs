﻿namespace UniGame.UiSystem.Runtime.WindowStackControllers
{
    using UniGame.Attributes;
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using Backgrounds.Abstract;
    using Core.Runtime;
    using R3;
    using ViewSystem.Runtime;
    using ViewSystem.Runtime.WindowStackControllers.Abstract;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    public class ViewSystemLayoutComponent : MonoBehaviour, IViewLayout
    {
        #region inspector

        [SerializeField]
        public Canvas layoutCanvas;

        [SerializeField]
        public BackgroundFactory backgroundFactory;

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
        
        public Observable<IView> OnHidden => LayoutController.OnHidden;
        
        public Observable<IView> OnShown => LayoutController.OnShown;
        
        public Observable<IView> OnClosed => LayoutController.OnClosed;
        
        public Observable<Type>  OnIntent => LayoutController.OnIntent;

        public Observable<IView> OnBeginHide  => LayoutController.OnBeginHide;
        
        public Observable<IView> OnBeginShow  => LayoutController.OnBeginShow;
        
        public ILifeTime LifeTime => LayoutController.LifeTime;
        
        #region public methods

        public void Dispose() => LayoutController.Dispose();

        public bool Contains(IView view) => LayoutController.Contains(view);

        public void HideAll() => LayoutController.HideAll();

        public ReadOnlyReactiveProperty<IView> ActiveView => LayoutController.ActiveView;

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