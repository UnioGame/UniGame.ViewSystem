using System.Collections;
using UniGreenModules.UniRoutine.Runtime;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.Core.Runtime.Rx;
    using UniGreenModules.UniGame.UiSystem.Runtime.Extensions;
    using UniRx;
    using UnityEngine;

    public abstract class UiView<TViewModel> :
        MonoBehaviour, IView
        where TViewModel : class, IViewModel
    {

        private LifeTimeDefinition lifeTimeDefinition = new LifeTimeDefinition();
        private RecycleReactiveProperty<Unit> closeReactiveValue = new RecycleReactiveProperty<Unit>();
        private IViewElementFactory viewFactory;

        /// <summary>
        /// ui element visibility status
        /// </summary>
        protected BoolRecycleReactiveProperty visibility = new BoolRecycleReactiveProperty();

        /// <summary>
        /// model container
        /// </summary>
        private ReactiveProperty<TViewModel> viewModel = new ReactiveProperty<TViewModel>();

        #region public properties

        public TViewModel Model => viewModel.Value;

        /// <summary>
        /// Is View Active
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsActive => visibility;

        /// <summary>
        /// View LifeTime
        /// </summary>
        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        /// <summary>
        /// views factor
        /// </summary>
        public IViewElementFactory ViewFactory => viewFactory;
        
        public IObservable<Unit> OnHidden => visibility.Where(x => !x).AsUnitObservable();

        public IObservable<Unit> OnShown => visibility.Where(x => x).AsUnitObservable();

        public IObservable<Unit> OnClosed => closeReactiveValue;
        
        #endregion

        #region public methods

        public void Initialize(IViewModel model, IViewElementFactory factory)
        {
            //restart view lifetime
            lifeTimeDefinition.Release();

            //save model as context data
            if (model is TViewModel modelData)
            {
                this.viewModel.Value = modelData;
            }
            else
            {
                throw new ArgumentException($"VIEW: {name} wrong model type. Target type {typeof(TViewModel).Name} : model Type {model?.GetType().Name}");
            }

            this.viewFactory = factory;

            //bind model lifetime to local
            var modelLifeTime = model.LifeTime;
            modelLifeTime.AddCleanUpAction(Close);

            //terminate model when view closed
            lifeTimeDefinition.AddDispose(model);
            lifeTimeDefinition.AddCleanUpAction(() => factory = null);
            lifeTimeDefinition.AddCleanUpAction(() => visibility.Release());
            lifeTimeDefinition.AddCleanUpAction(() => {
                closeReactiveValue.Value = Unit.Default;
                closeReactiveValue.Release();
            });

            //custom initialization
            OnInitialize(modelData);

        }

        /// <summary>
        /// show active view
        /// </summary>
        public virtual void Show() => visibility.Value = true;

        /// <summary>
        /// hide view without release it
        /// </summary>
        public virtual void Hide() => visibility.Value = false;

        /// <summary>
        /// end of view lifetime
        /// </summary>
        public void Close()
        {
            if (lifeTimeDefinition.IsTerminated) return;
            OnClose().Execute().
                AddTo(LifeTime);
        }
        
        public void Destroy() => lifeTimeDefinition.Terminate();

        /// <summary>
        /// bind source stream to view action
        /// with View LifeTime context
        /// </summary>
        public UiView<TViewModel> BindTo<T>(IObservable<T> source, Action<T> action) => this.Bind(source, action);

        #endregion

        /// <summary>
        /// custom initialization methods
        /// </summary>
        protected virtual void OnInitialize(TViewModel model) { }

        /// <summary>
        /// view closing process
        /// windows auto terminated on close complete
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnClose()
        {
            //wait until user defined closing operation complete
            yield return OnCloseProgress();
            lifeTimeDefinition.Terminate();
        }
        
        /// <summary>
        /// hide process
        /// </summary>
        private IEnumerator OnHiding()
        {
            //wait until user defined closing operation complete
            yield return OnHidingProgress();
            
        }

        /// <summary>
        /// close continuation
        /// </summary>
        protected virtual IEnumerator OnCloseProgress()
        {
            yield break;
        }

        /// <summary>
        /// showing continuation
        /// </summary>
        protected virtual IEnumerator OnShowProgress()
        {
            yield break;
        }
        
        /// <summary>
        /// hiding continuation
        /// </summary>
        protected virtual IEnumerator OnHidingProgress()
        {
            yield break;
        }
        
        private void OnDestroy()
        {
            GameLog.Log($"View {name} Destroyed");
            Close();
        }



    }
}