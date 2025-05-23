using UniGame.Addressables.Reactive;
using UniGame.AddressableTools.Runtime;

namespace UniGame.Rx.Runtime.Extensions
{
    using System;
    using System.Runtime.CompilerServices;
    using AddressableTools.Runtime.AssetReferencies;
    using Cysharp.Threading.Tasks;
    using TMPro;
    using global::UniGame.Core.Runtime;
    using global::UniGame.ViewSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Extensions;
    using UnityEngine;
    using UnityEngine.UI;
    using global::UniGame.Localization.Runtime.UniModules.UniGame.Localization.Runtime;
    using global::UniGame.UiSystem.Runtime;
    using UniGame.Runtime.Common;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.Core.Runtime.DataFlow.Extensions;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniRx;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Components;
    using Object = UnityEngine.Object;

    public static class ViewBindingExtension
    {
        #region ugui extensions

        public static TView Bind<TView>(this TView view, LocalizedString source, Action<string> command)
            where TView : class, IView
        {
            return source == null ? view : view.Bind(source.AsObservable(), command);
        }

        public static TView Bind<TView>(this TView view, IObservable<LocalizedString> source, TextMeshProUGUI text)
            where TView : class, IView
        {
            return source == null 
                ? view 
                : view.Bind(source, x => view.Bind(x, text));
        }
        
        public static TView Bind<TView>(this TView view, LocalizedString source, TextMeshProUGUI text)
            where TView : class, IView
        {
            return source == null ? view : view.Bind(source.AsObservable(), text);
        }

        public static TView Bind<TView>(this TView view, LocalizeStringEvent source, TextMeshProUGUI text)
            where TView : class, IView
        {
            return source == null || source.StringReference == null || source.StringReference.IsEmpty
                ? view
                : view.Bind(source.StringReference.AsObservable(), text);
        }

        public static TView Bind<TView>(this TView view, LocalizeStringEvent source, Action<string> command)
            where TView : class, IView
        {
            return source == null || source.StringReference == null || source.StringReference.IsEmpty
                ? view
                : view.Bind(source.StringReference.AsObservable(), command);
        }

        public static TView BindAsync<TView>(this TView view, AssetReferenceT<Sprite> source, Image image)
            where TView : class, IView
        {
            if (source.RuntimeKeyIsValid() == false) return view;
            return !image ? view : view.Bind(source.ToObservable(view.LifeTime), image);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TView Bind<TView>(this TView view, IObservable<AssetReferenceT<Sprite>> source, Image image)
            where TView : class, IView
        {
            if (image == null) return view;
            
            return !image
                ? view
                : view.Bind(source, x => Bind(view, x, image));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TView Bind<TView>(this TView view, IObservable<AssetReference> source, Image image)
            where TView : class, IView
        {
            if (image == null) return view;
            
            return !image
                ? view
                : view.Bind(source, x => Bind(view, x, image));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TView Bind<TView>(this TView view, 
            IObservable<AddressableValue<Sprite>> source, Image image)
            where TView : class, IView
        {
            return view.Bind(source, x =>
                {
                    if (x?.reference == null) return;
                    Bind(view, x.reference, image);
                });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TView Bind<TView>(this TView view, AssetReferenceT<Sprite> source, Image image)
            where TView : class, IView
        {
            if (image == null) return view;
            image.Bind(source).Forget();
            return view;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async UniTask<TView> Bind<TView>(this TView view, AssetReference source, Image image)
            where TView : class, IView
        {
            if (image == null) return view;
            await image.Bind(source);
            return view;
        }
        
        public static async UniTask<bool> Bind<TValue>(
            this AssetReference reference,
            Action<TValue> action, 
            ILifeTime lifeTime)
            where TValue : Object
        {
            if(action == null) return false;
            
            var value = reference == null || !reference.RuntimeKeyIsValid()
                ? null
                : await reference.LoadAssetTaskAsync<TValue>(lifeTime);

            if(value == null) return false;
            action?.Invoke(value);
            return true;
        }

        public static async UniTask<bool> Bind(
            this Image image, 
            AssetReference reference)
        {
            if (image == null) return false;
            var lifeTime = image.GetAssetLifeTime();
            
            var sprite = reference == null || !reference.RuntimeKeyIsValid()
                ? null
                : await reference.LoadAssetTaskAsync<Sprite>(lifeTime);

            image.SetValue(sprite);
            
            return sprite != null;
        }
        
        public static async UniTask<bool> Bind(this Image image, AssetReferenceT<Sprite> reference)
        {
            if (image == null) return false;
            
            var lifeTime = image.GetAssetLifeTime();
            var sprite = reference == null || !reference.RuntimeKeyIsValid()
                ? null
                : await reference.LoadAssetTaskAsync(lifeTime);
            
            image.SetValue(sprite);
            return sprite != null;
        }
        
        
        public static TView BindAsync<TView>(this TView view, IObservable<AssetReferenceT<Sprite>> source, Image image)
            where TView : class, IView
        {
            if (image == null) return view;
            return !image
                ? view
                : view.Bind(source, async x =>
                {
                    var sprite = x == null || !x.RuntimeKeyIsValid() 
                        ? null
                        : await x.LoadAssetTaskAsync(view.LifeTime);
                    image.sprite = sprite;
                });
        }

        public static TView Bind<TView, TValue>(this TView view, AssetReferenceT<TValue> source, Action<TValue> action)
            where TView : class, IView
            where TValue : Object
        {
            if (source.RuntimeKeyIsValid() == false || action == null) return view;
            AddressableAction(view.LifeTime, source, action).Forget();
            return view;
        }

        private static async UniTask AddressableAction<TValue>(ILifeTime lifeTime, AssetReferenceT<TValue> source,
            Action<TValue> action)
            where TValue : Object
        {
            if (action == null || lifeTime.IsTerminated)
                return;
            var value = await source.LoadAssetTaskAsync(lifeTime);
            action(value);
        }

        #endregion

        public static async UniTask<TView> Bind<TView, TModel>(this TView view, TModel model, IView target)
            where TView : class, IView
            where TModel : IViewModel
        {
            await target.Initialize(model);
            return view;
        }


        public static IDisposable Bind(this IObservable<string> source, TextMeshPro text)
        {
            return source.Subscribe(x => text.SetValue(x));
        }
        
        public static IDisposable Bind(this IObservable<string> source, TextMeshProUGUI text)
        {
            return source.Subscribe(x => text.SetValue(x));
        }

        public static TView Bind<TView>(this TView view, IObservable<Sprite> source, Button button)
            where TView : ILifeTimeContext
        {
            if (!button || !button.image)
                return view;

            return view.Bind(source, x => button.image.SetValue(x));
        }

        public static TView Bind<TView>(this TView view, IObservable<string> source, TextMeshProUGUI text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source, x => text.SetValue(x));
        }

        public static TView Bind<TView, TValue>(this TView view,
            IObservable<TValue> source,
            Func<TValue, string> format, TextMeshProUGUI text)
            where TView : ILifeTimeContext
        {
            var stringObservable = source.Select(format);
            return view.Bind(stringObservable, text);
        }
        
        public static TView Bind<TView>(this TView view, IObservable<int> source, int value)
            where TView : ILifeTimeContext
        {
            return view.Bind(source, x => value = x);
        }

        public static TView Bind<TView>(this TView view, IObservable<string> source, TextMeshPro text)
            where TView : ILifeTimeContext
        {
            if (!text) return view;
            return view.Bind(source, x => text.text = x);
        }

        public static TView Bind<TView>(this TView view, IObservable<int> source, TextMeshPro text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source, x => text.text = x.ToStringFromCache());
        }

        public static TView Bind<TView>(this TView view, IObservable<string> source, TMP_InputField text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source,x => text.SetValue(x));
        }
        
        public static TView Bind<TView>(this TView view,TMP_InputField source, IReactiveProperty<int> value)
            where TView : ILifeTimeContext
        {
            if (source == null) return view;
            var observable = source.onValueChanged.AsObservable();
            return view.Bind(observable, x =>
            {
                int.TryParse(x, out var result);
                value.Value = result;
            });
        }
        
        public static TView Bind<TView>(this TView view,TMP_InputField source, IObserver<string> value)
            where TView : ILifeTimeContext
        {
            if (source == null) return view;
            var observable = source.onValueChanged.AsObservable();
            return view.Bind(observable, value);
        }
        
        public static TView Bind<TView>(this TView view,TMP_InputField source, 
            IReactiveProperty<string> value,bool initDefault = true)
            where TView : ILifeTimeContext
        {
            if (source == null || value == null) return view;
            if(initDefault)
                value.Value = source.text;
            
            var observable = source.onValueChanged.AsObservable();
            return view.Bind(observable, value);
        }
        
        public static TView Bind<TView>(this TView view,TMP_InputField source, Action<string> value)
            where TView : ILifeTimeContext
        {
            if (source == null || value == null) return view;
            var observable = source.onValueChanged.AsObservable();
            return view.Bind(observable, value);
        }
        
        public static TView Bind<TView>(this TView view,TMP_InputField source, Action value)
            where TView : ILifeTimeContext
        {
            if (source == null || value == null) return view;
            var observable = source.onValueChanged.AsObservable();
            return view.Bind(observable, value);
        }
        
        public static TView Bind<TView>(this TView view, IObservable<int> source, TMP_InputField text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source,x => text.SetValue(x.ToStringFromCache()));
        }
        
        public static TView Bind<TView>(this TView view, IObservable<float> source, TMP_InputField text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source,x => text.SetValue(x.ToStringFromCache()));
        }
        
        public static TView Bind<TView>(this TView view, IObservable<Color> source, TextMeshProUGUI text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source,x => text.SetValue(x));
        }
        
        public static TView Bind<TView>(this TView view, IObservable<Color> source, TextMeshPro text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source,x => text.SetValue(x));
        }
        
        public static TView Bind<TView>(this TView view, IObservable<Color> source, TMP_InputField text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source,x => text.SetValue(x));
        }
        
        public static TView Bind<TView>(this TView view, IObservable<Color> source, Button button)
            where TView : ILifeTimeContext
        {
            return view.Bind(source,x => button.SetValue(x));
        }

        public static TView Bind<TView>(this TView view, IObservable<int> source, TextMeshProUGUI text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source, x => text.SetValue(x.ToStringFromCache()));
        }
        
        public static TView Bind<TView>(this TView view, IObservable<float> source, TextMeshProUGUI text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source, x => text.SetValue(x.ToStringFromCache()));
        }
        
        public static TView Bind<TView>(this TView view, IObservable<float> source, TextMeshPro text)
            where TView : ILifeTimeContext
        {
            return view.Bind(source, x => text.SetValue(x.ToStringFromCache()));
        }

        public static TView Bind<TView>(this TView view, IObservable<Sprite> source, Image image)
            where TView : ILifeTimeContext
        {
            return !image
                ? view
                : view.Bind(source,x => image.SetValue(x));
        }
        
        public static TView Bind<TView>(this TView view, IObservable<Sprite> source, RawImage image)
            where TView : ILifeTimeContext
        {
            return !image
                ? view
                : view.Bind(source.Where(x => x != null),x => image.SetValue(x));
        }

        public static TView Bind<TView>(this TView sender, IObservable<Color> source, Image image)
            where TView : ILifeTimeContext
        {
            return image == null
                ? sender
                : sender.Bind(source,x => image.SetValue(x));
        }
        
        public static TView Bind<TView>(this TView sender, IObservable<Color> source, RawImage image)
            where TView : ILifeTimeContext
        {
            return image == null
                ? sender
                : sender.Bind(source,x => image.SetValue(x));
        }

        public static TView Bind<TView>(this TView view, IObservable<bool> source, CanvasGroup group)
            where TView : ILifeTimeContext
        {
            if (!group) return view;
            return view.Bind(source, x => group.alpha = x ? 1 : 0);
        }

        public static TView Bind<TView>(this TView view, IObservable<Texture> source, RawImage image)
            where TView : ILifeTimeContext
        {
            return !image
                ? view
                : view.Bind(source.Where(x => x != null), x => image.texture = x);
        }

        public static TView Bind<TView>(this TView view, IObservable<bool> source, Toggle toggle)
            where TView : ILifeTimeContext
        {
            return !toggle ? view : view.Bind(source, x => toggle.isOn = x);
        }


        public static TView Bind<TView, TValue>(this TView sender, IObservable<TValue> source, Button command)
            where TView : ILifeTimeContext
        {
            return command == null 
                ? sender 
                : sender.Bind(source, x => command.onClick?.Invoke());
        }

        public static TView Bind<TView>(this TView sender, IObservable<float> source, Slider slider)
            where TView : ILifeTimeContext
        {
            return source == null || slider == null
                ? sender
                : sender.Bind(source, x => slider.value = x);
        }
        
        public static TView Bind<TView>(this TView sender,IObservable<float> maxValue, IObservable<float> source, Slider slider)
            where TView : ILifeTimeContext
        {
            if (source == null || slider == null) return sender;
            sender.Bind(maxValue,x => slider.maxValue = x);    
            return sender.Bind(source,slider);
        }
        
        public static TView Bind<TView>(this TView sender,IObservable<int> maxValue, IObservable<int> source, Slider slider)
            where TView : ILifeTimeContext
        {
            if (source == null || slider == null) return sender;
            sender.Bind(maxValue,x => slider.maxValue = x);    
            return sender.Bind(source,slider);
        }
        
        public static TView Bind<TView>(this TView sender, IObservable<int> source, Slider slider)
            where TView : ILifeTimeContext
        {
            return source == null || slider == null
                ? sender
                : sender.Bind(source, x => slider.value = x);
        }

        public static TView Bind<TView>(this TView sender, Button source, Action command)
            where TView : ILifeTimeContext
        {
            if (source == null || sender == null) return sender;
            var observable = source.OnClickAsObservable();
            return sender.Bind(observable, command);
        }
        
        public static TView Bind<TView>(this TView sender, Slider source, Action command)
            where TView : ILifeTimeContext
        {
            if (source == null || sender == null) return sender;
            var observable = source.OnValueChangedAsObservable();
            return sender.Bind(observable, command);
        }

        public static TView Bind<TView>(this TView sender, Button source, Action<Unit> command)
            where TView : ILifeTimeContext
        {
            return sender.Bind(source, () => command(Unit.Default));
        }

        public static IDisposable Bind(this LocalizedString source, TextMeshProUGUI text, int frameThrottle = 1)
        {
            return source.Bind(x => text.SetValue(x),frameThrottle);
        }
        
                
        public static T BindLifeTime<T>(this T sender, Action disableAction)
            where T : ILifeTimeContext
        {
            if (sender == null) return sender;
            sender.LifeTime.AddCleanUpAction(disableAction);
            return sender;
        }
        
        public static T BindViewLifeTime<T>(this T sender, Action disableAction)
            where T : IView
        {
            if (sender == null) return sender;
            sender.ViewLifeTime.AddCleanUpAction(disableAction);
            return sender;
        }
        
        public static TView Bind<TView>(this TView sender,
            Button source,
            ISignalValueProperty<bool> value)
            where TView : ILifeTimeContext
        {
            return source == null
                ? sender
                : sender.Bind(source.OnClickAsObservable(), () => value.SetValue(true));
        }
        
                
        public static T Bind<T>(this T sender,
            IObservable<bool> source,Image image, Sprite on, Sprite off)
            where T : ILifeTimeContext
        {
            var sourceObservable = source.Select(x => x ? on : off);
            return Bind(sender, sourceObservable, image);
        }
        
        public static T Bind<T>(this T sender,
            IObservable<bool> source,Button image, Sprite on, Sprite off)
            where T : ILifeTimeContext
        {
            var sourceObservable = source.Select(x => x ? on : off);
            return Bind(sender, sourceObservable, image);
        }
        
        public static TView Bind<TView>(this TView sender,
            IObservable<Unit> source,
            ISignalValueProperty<bool> value)
            where TView : ILifeTimeContext
        {
            return source == null
                ? sender
                : sender.Bind(source, () => value.SetValue(true));
        }
        
        public static TView Bind<TView>(this TView sender,
            IObservable<bool> source,
            ISignalValueProperty<bool> value)
            where TView : ILifeTimeContext
        {
            return source == null
                ? sender
                : sender.Bind(source, () => value.SetValue(true));
        }
        
        public static TView Bind<TView>(this TView sender,
            Toggle source,
            ISignalValueProperty<bool> value)
            where TView : ILifeTimeContext
        {
            return source == null
                ? sender
                : sender.Bind(source.OnValueChangedAsObservable(), value.SetValue);
        }

        public static TView Bind<TView>(this TView sender, Button source, Action<Unit> command, TimeSpan throttleTime)
            where TView : ILifeTimeContext
        {
            return source == null ? sender : sender.Bind(source, () => command(Unit.Default), throttleTime);
        }

        public static TView Bind<TView>(this TView sender, Button source, Action command, TimeSpan throttleTime)
            where TView : ILifeTimeContext
        {
            if (!source) return sender;

            var clickObservable = throttleTime.TotalMilliseconds <= 0
                ? source.OnClickAsObservable()
                : source.OnClickAsObservable().ThrottleFirst(throttleTime);

            return sender.Bind(clickObservable, command);
        }

        public static TView Bind<TView>(this TView view, IObservable<bool> source, Button button)
            where TView : ILifeTimeContext
        {
            return !button ? view : view.Bind(source, x => button.interactable = x);
        }

        public static TView Bind<TView>(this TView view, Button source, IReactiveCommand<Unit> command,
            int throttleInMilliseconds = 0)
            where TView : ILifeTimeContext
        {
            return !source
                ? view
                : Bind(view, source, () => command.Execute(Unit.Default),
                    TimeSpan.FromMilliseconds(throttleInMilliseconds));
        }

        public static TView Bind<TView>(this TView view,
            IObservable<bool> source,
            Image image)
            where TView : ILifeTimeContext
        {
            return image == null 
                ? view 
                : view.Bind(source, x => image.enabled = x);
        }
        
        public static TView Bind<TView>(this TView view,
            IObservable<bool> source,
            MonoBehaviour component)
            where TView : ILifeTimeContext
        {
            return component == null 
                ? view 
                : view.Bind(source, x => component.enabled = x);
        }
        
        public static TView Bind<TView>(this TView view,
            IObservable<bool> source,
            RawImage image)
            where TView : ILifeTimeContext
        {
            return image == null 
                ? view 
                : view.Bind(source, x => image.enabled = x);
        }

        public static TView Bind<TView>(this TView view, IObservable<Unit> source, IReactiveCommand<Unit> command)
            where TView : ILifeTimeContext
        {
            return view.Bind(source, x => command.Execute(Unit.Default));
        }
        
        public static TView Bind<TView,TData>(this TView view, IObservable<TData> source, IReactiveCommand<TData> command)
            where TView : ILifeTimeContext
        {
            return view.Bind(source, x => command.Execute(x));
        }

        public static TSource Bind<TSource>(this TSource view, Toggle source, IReactiveProperty<bool> value)
            where TSource : ILifeTimeContext
        {
            return !source ? view : view.Bind(source.OnValueChangedAsObservable(), value);
        }

        public static TSource Bind<TSource>(this TSource view, Toggle source, IReactiveCommand<bool> value)
            where TSource : ILifeTimeContext
        {
            if (source == null) return view;
            var observable = source.OnValueChangedAsObservable();
            return view.Bind(observable, value, view.LifeTime);
        }

        public static TView Bind<TView>(this TView view, Toggle source, Action<bool> value)
            where TView : ILifeTimeContext
        {
            return !source ? view : view.Bind(source.OnValueChangedAsObservable(), value);
        }

        public static TSource BindClose<TSource, TView>(
            this TSource view,
            TView source)
            where TSource : IView
            where TView : IView
        {
            source.CloseWith(view.LifeTime);
            return view;
        }

        public static TSource BindClose<TSource, T>(
            this TSource view,
            IView target)
            where TSource : IView
            where T : IViewModel
        {
            target.CloseWith(view.LifeTime);
            return view;
        }

        public static TSource Bind<TSource, T>(
            this TSource view,
            IObservable<T> source,
            ViewBase target,
            bool closeWith = false)
            where TSource : ViewBase
            where T : IViewModel
        {
            if (closeWith) view.BindClose(target);

            view.Bind(source, x => target.Initialize(x, view.Layout)
                .AttachExternalCancellation(view.ModelLifeTime.Token)
                .Forget());

            return view;
        }

        public static TSource BindToWindow<TSource>(
            this TSource view,
            IObservable<IViewModel> source,
            Type viewType)
            where TSource : ViewBase
        {
            return view.Bind(source, x => view.OpenAsWindowAsync(x, viewType));
        }

        public static TSource BindWhere<TSource, T>(
            this TSource view,
            Object indicator,
            IObservable<T> source,
            Action<T> target)
            where TSource : IView
        {
            return indicator != null ? view.Bind(source, target) : view;
        }
    }
}