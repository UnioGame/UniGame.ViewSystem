namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using AddressableTools.Runtime.AssetReferencies;
    using Rx.Runtime.Extensions;
    using TMPro;
    using UniGame.Runtime.Common;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.UI;

    public class ViewFieldBinder
    {
        public IView Bind(IView view, ref BindDataConnection bindData)
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
                case IObservable<Color> observable:
                    return Bind(view, observable, ref bindData);
                case IObservable<float> observable:
                    return Bind(view, observable, ref bindData);
                case IObservable<Texture> observable:
                    return Bind(view, observable, ref bindData);
                case IObservable<AssetReferenceT<Sprite>> observable:
                    return Bind(view, observable, ref bindData);
                case IObservable<AddressableValue<Sprite>> observable:   
                    return Bind(view, observable, ref bindData);
                case SignalValueProperty<bool> observable:   
                    return Bind(view, observable, ref bindData);
            }

            return view;
        }

        public IView Bind(IView view, IObservable<int> observable, ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;

            return targetValue switch
            {
                TextMeshProUGUI value => view.Bind(observable, value),
                TextMeshPro value => view.Bind(observable, value),
                TMP_InputField value => view.Bind(observable, value),
                _ => Bind<int>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view, IObservable<Color> observable, ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;

            return targetValue switch
            {
                Image value => view.Bind(observable, value),
                TextMeshProUGUI value => view.Bind(observable, value),
                TextMeshPro value => view.Bind(observable, value),
                TMP_InputField value => view.Bind(observable, value),
                Button value => view.Bind(observable, value),
                _ => Bind<Color>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view, IObservable<float> observable, ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;

            return targetValue switch
            {
                TextMeshProUGUI value => view.Bind(observable, value),
                TextMeshPro value => view.Bind(observable, value),
                TMP_InputField value => view.Bind(observable, value),
                Slider value => view.Bind(observable, value),
                _ => Bind<float>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view, IObservable<bool> observable, ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;

            return targetValue switch
            {
                Image value => view.Bind(observable, value),
                GameObject value => view.Bind(observable, value),
                RawImage value => view.Bind(observable, value),
                Toggle value => view.Bind(observable, value),
                CanvasGroup value => view.Bind(observable, value),
                _ => Bind<bool>(view, observable, ref bindData)
            };
        }

        public IView Bind<T>(IView view, IObservable<T> observable, ref BindDataConnection bindData)
        {
            if (observable == null) return view;
            
            var targetValue = bindData.targetValue;
            
            return targetValue switch
            {
                ISubject<T> value => view.Bind(observable, value),
                IReactiveCommand<T> value => view.Bind(observable, value),
                IReactiveProperty<T> value => view.Bind(observable, value),
                IObserver<T> value => view.Bind(observable, value),
                _ => view
            };
        }
        
        public IView Bind(IView view,IObservable<string> observable,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                TextMeshProUGUI value => view.Bind(observable, value),
                TextMeshPro value => view.Bind(observable, value),
                TMP_InputField value => view.Bind(observable, value),
                _ => Bind<string>(view, observable, ref bindData)
            };
        }

        public IView Bind(IView view,IObservable<Sprite> observable,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                Image image => view.Bind(observable, image),
                RawImage rawImage => view.Bind(observable, rawImage),
                _ => Bind<Sprite>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view,IObservable<AssetReferenceT<Sprite>> observable,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                Image image => view.Bind(observable, image),
                _ => Bind<AssetReferenceT<Sprite>>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view,IObservable<AddressableValue<Sprite>> observable,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                Image image => view.Bind(observable, image),
                _ => Bind<AddressableValue<Sprite>>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view,SignalValueProperty<bool> source,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                Button value => view.Bind(value, source),
                Toggle value => view.Bind(value, source),
                IObservable<Unit> value=> view.Bind(value,source),
                IObservable<bool> value=> view.Bind(value,source),
                _ => view
            };  
        }
        
        public IView Bind(IView view,IObservable<Texture> observable,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                RawImage rawImage => view.Bind(observable, rawImage),
                _ => Bind<Texture>(view, observable, ref bindData)
            };
        }
    }
}