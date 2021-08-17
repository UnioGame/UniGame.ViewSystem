using UniModules.UniGame.Core.Runtime.Attributes;
using UnityEngine;

namespace UniGame.UiSystem.Runtime.WindowStackControllers
{
    using System;
    using System.Collections.Generic;
    using Backgrounds.Abstract;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniModules.UniGame.UISystem.Runtime.WindowStackControllers.Abstract;
    using UniRx;

    public class ViewsLayoutAsset : MonoBehaviour, IViewLayout
    {
        #region inspector

        [SerializeField]
        public Canvas _layoutCanvas;

        [SerializeField]
        public BackgroundFactory _backgroundFactory;

        [SerializeReference]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Required]
#endif
        [AssetFilter(typeof(ViewLayoutType))]
        public ViewLayoutType _layoutBehaviourFactory;

        #endregion

        private readonly Lazy<IViewLayout> _layout;

        public IViewLayout LayoutController => _layout.Value;

        public Transform Layout => LayoutController.Layout;
        
        public IObservable<IView> OnHidden => LayoutController.OnHidden;
        
        public IObservable<IView> OnShown => LayoutController.OnShown;
        
        public IObservable<IView> OnClosed => LayoutController.OnClosed;
        
        public IObservable<Type>  OnIntent => LayoutController.OnIntent;

        public IObservable<IView> OnBeginHide  => LayoutController.OnBeginHide;
        
        public IObservable<IView> OnBeginShow  => LayoutController.OnBeginShow;
        
        public ILifeTime LifeTime => LayoutController.LifeTime;
        
        #region public methods

        public ViewsLayoutAsset()
        {
            _layout = new Lazy<IViewLayout>(Create);
        }

        public void Dispose() => LayoutController.Dispose();

        public bool Contains(IView view) => LayoutController.Contains(view);

        public void HideAll()
        {
            LayoutController.HideAll();
        }

        public IReadOnlyReactiveProperty<IView> ActiveView => LayoutController.ActiveView;

        public void ApplyIntent<T>() where T : IView
        {
            LayoutController.ApplyIntent<T>();
        }

        public void Push(IView view)
        {
            LayoutController.Push(view);
        }

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
            if (_layoutCanvas != null)
                _layoutCanvas.enabled = false;
        }

        public void Resume()
        {
            if (_layoutCanvas != null)
                _layoutCanvas.enabled = true;
        }

        #endregion

        #region private methods

        protected virtual IViewLayout Create()
        {
            IBackgroundView backgroundView = null;
            if (_backgroundFactory != null) {
                backgroundView = _backgroundFactory.Create(_layoutCanvas?.transform);
                backgroundView.Hide();
            }
            
            if(_layoutBehaviourFactory == null)
                throw new NullReferenceException(nameof(_layoutBehaviourFactory));

            return _layoutBehaviourFactory.Create(_layoutCanvas.transform, backgroundView);
        }

        protected void OnDestroy()
        {
            if(_layout.IsValueCreated)
                _layout.Value.Dispose();
        }

        #endregion
    }
}