using UnityEngine;

namespace UniGame.UiSystem.ModelViews.Runtime.Flow
{
    using System;
    using UiSystem.Runtime.Abstracts;
    

    public static class FlowCommands 
    {
    
        public static IObservable<IViewModel> ShowAs<TView>(this IViewModel viewModel)
            where TView : IView
        {
            return ShowAs(viewModel,typeof(TView));
        }
        
        public static IObservable<IViewModel> ShowAs(this IViewModel viewModel, Type viewType)
        {
            
            return null;
        }
        
        public static IObservable<IViewModel> Show(this IViewModel viewModel)
        {
            return null;
        }
        
        public static IObservable<IViewModel> Hide(this IViewModel viewModel)
        {
            return null;
        }
        
        public static IObservable<IViewModel> Close(this IViewModel viewModel)
        {
            return null;
        }
        
        public static IObservable<IViewModel> Kill(this IViewModel viewModel)
        {
            return null;
        }
        

    }
}
