# UniGame.ViewSystem

MVVM View System for Unity3D

**Odin Inspector Asset recommended to usage with this Package (https://odininspector.com)**


- [Overview](#overview)
- [Getting Started](#getting-started)
- [View System Settings](#view-system-settings)
  - [SetUp Views Locations](#setup-views-locations) 
  - [Addressable Support](#addressable-support) 
  - [Nested View Sources](#nested-view-sources) 
  - [Layouts Control](#layouts-control)
  - [Settings Rebuild](#settings-rebuild)
- [Skins Support](#skins-support)
- [Pooling Support](#skins-support)
- [API References](#api-references)
  - [Views & ViewModels](#views-&-viewmodels)
  - [Reactive Binding](#reactive-binding)
  - [Context Views](#context-views)
- [Examples](#examples)
- [License](#license)

## Overview

- support base principles of MVVM concepts.
- support ui skins out of the box
- based on Unity Addressables Resources
- handle Addressables Resource lifetime

## Getting Started

Add to your project manifiest by path [%UnityProject%]/Packages/manifiest.json new Scope:

```json
{
  "scopedRegistries": [
   {
      "name": "UniGame",
      "url": "http://packages.unigame.pro:4873/",
      "scopes": [
        "com.unigame"
      ]
    }
  ],
}

```

and now install via Package Manager

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Readme/Assets/package_manager.png)

## Getting Started

- Create View System Asset

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/view_asset_prefab.png)


## View System Settings

### Create Settings

![](https://i.gyazo.com/15833fe0019b9570d68cab6ba20d3df6.png)


### SetUp Views Locations

Here you can initialize locations of views

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/views_setup.png)

For skinned views. Skin name of view equal to it's parent folder. Your project views prefabs structure involve direct mapping into its skins 

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/skin-folders-names.png)


### Addressable Support

For now All views load at runtime through  <a href="https://docs.unity3d.com/Packages/com.unity.addressables@latest">Unity Addressable Asset system</a>

By default if your views not registered as Addressable Asset when View System automatically register it into new Addressable Group With name equal to its ViewsSettings Source Name

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/view-settings-source-name1.png)

You can enable Addressable Group Name override:

- Enable "Apply Addressable Group" option
- SetUp new views group name

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/views-addressable-override.png)

### Nested View Sources

View System Support additional "nested" view settings sources. All view from that sources will be registered into main View System when it loaded. All nested view settings loads async. If that't source must be loaded before the View System will be available its possible activate "Await Loading" option.

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/nested-settings-await.png)

### Layouts Control

"Layout Flow Control" asset control views behaviours between all layouts. View System support two flow behaviour out from the box.

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/views-flow-control.png)

- DefaultFlow 

Base flow controller with auto closing screens/windows when active scene changed

- SingleViewFlow

More complex flow with support 'IScreenSuspendingWindow' api. If View with 'IScreenSuspendingWindow' is open, when all current screens wills suspend and resume after it closed.

### Settings Rebuild


You can manualy trigger rebuild:

- Rebuild Command

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/view-settings-rebuild.png)

- For All Settings

![](https://i.gyazo.com/df803c28a8a9feb702cda99734cb9288.png)

- For target settings asset from inspector context menu

![](https://i.gyazo.com/7df8670d31e77df4c8f69bc2e7da9d92.png)


## Skins Support
Different flavours of the same view type can be created by utilizing skins. When a skin tag is provided on view creation the corresponding skin is instantiated (if it's been registered prior to it). Skin tag can be provided as a string or a variable of SkinId type (which allows choosing one of the registered tags from a dropdown list and implicitly converts it to a string)
```cs
void ExampleFunc(SkinId largeDemoView) {
  await gameViewSystem.OpenWindow<DemoView>(ViewModel, "SmallDemoView");
  await gameViewSystem.OpenWindow<DemoView>(ViewModel, largeDemoView);
}
```
### Skins via folders
Place views of the same type in separate folders and add them to the UI Views Skin Folders list in view system settings. After rebuilding the views will be added to the views registry with folder names as their skin tags
![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/skin-folders-names.png)

### Skins via component
Add View Skin Component to a prefab to turn it into a skin. To add a new skin tag enter it into Skin Tag Name field and press Invoke, an existing tag can be chosen from the Skin Tag dropdown list. No need to specify skin folders in view system settings
![image](https://user-images.githubusercontent.com/72013166/126639139-974d1458-7c14-490d-8b6a-4f3c1de33fa5.png)
![image](https://user-images.githubusercontent.com/72013166/126639300-bcc028a7-5070-4e78-9495-6d76c0ffc3b1.png)


## Pooling Support

## API References


### Views & ViewModels




### Reactive Binding

All base Bind extensions use ViewModelLifetime" thats allow auto disconnect from data streams when ViewModel changed

Binding extensions allow you easy connect you view and data sources with rich set flow syntax and support Rx method and async/await semantics

#### Bind To UGUI 

Help methods to direct bind unity UGUI types to data streams

- Button methods

Bind Button to model action

```cs

public Button openChest;

[Serializable]
public class WindowViewModel : ViewModelBase
{
    public ReactiveCommand checkAction = new ReactiveCommand();
    public IReactiveCommand<Unit> ChestAction => checkAction;
}

protected override UniTask OnViewInitialize(WindowViewModel model)
{
    this.Bind(openChest,model.ChestAction);
    
    return UniTask.CompletedTask;
}

```

Bind Model to Button invoke


```cs

public Button openChest;

[Serializable]
public class WindowViewModel : ViewModelBase
{
    public ReactiveCommand checkAction = new ReactiveCommand();
    public IReactiveCommand<Unit> ChestAction => checkAction;
}

protected override UniTask OnViewInitialize(WindowViewModel model)
{
    this.Bind(model.ChestAction,openChest);
    
    return UniTask.CompletedTask;
}

```

- TextMeshPro methods

```cs

[Serializable]
public class WindowViewModel : ViewModelBase
{
    public ReactiveProperty<string> label = new ReactiveProperty<string>();
    public ReactiveProperty<string> value = new ReactiveProperty<string>();
    
    public IObservable<string> Label => label;
    public IObservable<string> Value => value;
}

public TextMeshProUGUI label;
public TextMeshProUGUI value;

protected override UniTask OnViewInitialize(WindowViewModel model)
{
    this.Bind(model.Label,label)
        .Bind(model.Value,value);
    
    return UniTask.CompletedTask;
}

```

- Image methods

- LocaliztionString methods

#### Behaviour bindings

Allow you call show/hide/close and another actions with when views/data streams events occurs

## Examples

All examples can be found here:

https://github.com/UniGameTeam/UniGame.ViewSystem.Examples

### Item List View

![](https://github.com/UniGameTeam/UniGame.UISystem/blob/master/Readme/Assets/ui_list_demo.gif)

### Localization View

![](https://github.com/UniGameTeam/UniGame.UISystem/blob/master/Readme/Assets/localization_example.gif)

### Nested Views Sources

![](https://github.com/UniGameTeam/UniGame.UISystem/blob/master/Readme/Assets/nested_sources.png)

### View Skin loading

![](https://github.com/UniGameTeam/UniGame.UISystem/blob/master/Readme/Assets/skins_views.gif)


## License

<a href="https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/LICENSE">MIT</a>
