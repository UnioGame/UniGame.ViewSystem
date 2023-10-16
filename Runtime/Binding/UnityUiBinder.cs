namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Reflection;
    using AddressableTools.Runtime.AssetReferencies;
    using Rx.Runtime.Extensions;
    using TMPro;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.UI;

    public class UnityUiBinder : IViewBinder
    {
        public IView BindField(IView view,ref FieldBindData bindData)
        {
            var sourceValue = bindData.sourceValue;
            var targetValue = bindData.targetValue;
            
            if (sourceValue == null || targetValue == null) return view;

            switch (sourceValue)
            {
                case IObservable<Sprite> observable:
                    return Bind(view, observable, ref bindData);
                case IObservable<Unit> observable:
                    return Bind<Unit>(view, observable, ref bindData);
                case IObservable<string> observable:
                    return Bind(view, observable, ref bindData);
                case IObservable<bool> observable:
                    return Bind(view, observable, ref bindData);
                case IObservable<int> observable:
                    return Bind(view, observable, ref bindData);
                case IObservable<float> observable:
                    break;
                case IObservable<Texture> observable:
                    break;
                case IObservable<AssetReferenceT<Sprite>> observable:
                    break;
                case IObservable<AddressableValue<Sprite>> observable:   
                    break;
            }

            return view;
        }

        public IView BindMethod(IView view, object modelField, MethodInfo viewMethod)
        {
            return view;
        }
        
        public IView Bind(IView view, IObservable<int> observable, ref FieldBindData bindData)
        {
            var targetValue = bindData.targetValue;
            
            switch (targetValue)
            {
                case TextMeshProUGUI value:
                    return view.Bind(observable, value);
                case TextMeshPro value:
                    return view.Bind(observable, value);
                case TMP_InputField value:
                    return view.Bind(observable, value);
            }
            
            return view;
        }
        
        public IView Bind(IView view, IObservable<bool> observable, ref FieldBindData bindData)
        {
            var targetValue = bindData.targetValue;
            
            switch (targetValue)
            {
                case Image value:
                    return view.Bind(observable, value);
                case GameObject value:
                    return view.Bind(observable, value);
                case RawImage value:
                    return view.Bind(observable, value);
                case Toggle value:
                    return view.Bind(observable, value);
            }
            
            return view;
        }

        public IView Bind<T>(IView view, IObservable<T> observable, ref FieldBindData bindData)
        {
            var targetValue = bindData.targetValue;
            
            switch (targetValue)
            {
                case ISubject<T> value:
                    return view.Bind(observable, value);
                case IReactiveCommand<T> value:
                    return view.Bind(observable, value);
                case IReactiveProperty<T> value:
                    return view.Bind(observable, value);
            }
            
            return view;
        }
        
        public IView Bind(IView view,IObservable<string> observable,ref FieldBindData bindData)
        {
            var targetValue = bindData.targetValue;
            switch (targetValue)
            {
                case TextMeshProUGUI value:
                    return view.Bind(observable, value);
                case TextMeshPro value:
                    return view.Bind(observable, value);
                case TMP_InputField value:
                    return view.Bind(observable, value);
            }
            return view;
        }

        public IView Bind(IView view,IObservable<Sprite> observable,ref FieldBindData bindData)
        {
            var targetValue = bindData.targetValue;
            switch (targetValue)
            {
                case Image image:
                    return view.Bind(observable, image);
                case RawImage rawImage:
                    return view.Bind(observable, rawImage);
            }
            return view;
        }
    }
}