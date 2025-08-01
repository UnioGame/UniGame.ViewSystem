﻿namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Linq;
    using Backgrounds.Abstract;
    using R3;
    using ViewSystem.Runtime;
     
    using UnityEngine;

    [Serializable]
    public class SingleViewLayout : ViewLayout
    {
        private IBackgroundView _background;
        private IView _activeView;

        public void Initialize(Transform layout, IBackgroundView background)
        {
            _background = background;
            Layout      = layout;

            OnClosed.Where(x => x == _activeView).
                Subscribe(HideView).
                AddTo(LifeTime);
            
            OnBeginHide.Where(x => x == _activeView).
                Subscribe(HideView).
                AddTo(LifeTime);

            OnBeginShow.Where(x => x != _activeView).
                Subscribe(ActivateView).
                AddTo(LifeTime);
        }

        protected override void OnBeforeClose(IView view)
        {
            if (view == _activeView) {
                HideView(view);
            }
        }

        private void HideView(IView view)
        {
            //mark active view as empty
            _activeView = null;

            var lastView = Views.LastOrDefault(x => x != view);
            //empty view stack or only active
            if (lastView == null) 
            {
                _background?.Hide();
            }
        }

        private void ActivateView(IView view)
        {
            if(_activeView != null && view != _activeView)
                _activeView.Close();
            
            _activeView = view;

            //update top of stack
            Remove(view);
            Add(view);
            
            //show view if it inactive
            if(view.IsVisible.CurrentValue == false)
                view.Show();
            
            //show background if exists
            _background?.Show();
        }
    }
}