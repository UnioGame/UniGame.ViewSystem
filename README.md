# UniGame.ViewSystem

MVVM View System for Unity3D

**Odin Inspector Asset recommended to usage with this Package (https://odininspector.com)**


- [UniGame.ViewSystem](#unigameviewsystem)
  - [Overview](#overview)
  - [Getting Started](#getting-started)
  - [Getting Started](#getting-started-1)
  - [View System Settings](#view-system-settings)
    - [Create Settings](#create-settings)
    - [SetUp Views Locations](#setup-views-locations)
    - [Addressable Support](#addressable-support)
    - [Nested View Sources](#nested-view-sources)
    - [Layouts Control](#layouts-control)
    - [Settings Rebuild](#settings-rebuild)
  - [Skins Support](#skins-support)
    - [Skins via folders](#skins-via-folders)
    - [Skins via component](#skins-via-component)
    - [Custom Views Factory](#custom-views-factory)
      - [Enable Zenject DI Support](#enable-zenject-di-support)
  - [Pooling Support](#pooling-support)
  - [API References](#api-references)
    - [Views \& ViewModels](#views--viewmodels)
      - [View Lifetime Management](#view-lifetime-management)
        - [Two-Level LifeTime System](#two-level-lifetime-system)
          - [Example: ViewLifeTime vs ModelLifeTime](#example-viewlifetime-vs-modellifetime)
        - [ViewLifeTime vs ModelLifeTime](#viewlifetime-vs-modellifetime)
        - [Automatic Subscription Cleanup](#automatic-subscription-cleanup)
        - [View Status States](#view-status-states)
    - [Reactive Binding](#reactive-binding)
      - [Zero-Allocation Binding (Performance Optimization)](#zero-allocation-binding-performance-optimization)
      - [Bind To UGUI](#bind-to-ugui)
      - [Behaviour bindings](#behaviour-bindings)
  - [Examples](#examples)
    - [Item List View](#item-list-view)
    - [Localization View](#localization-view)
    - [Nested Views Sources](#nested-views-sources)
    - [View Skin loading](#view-skin-loading)
    - [Real Project Demo](#real-project-demo)
  - [License](#license)

## Overview

- support base principles of MVVM concepts.
- support ui skins out of the box
- based on Unity Addressables Resources
- handle Addressables Resource lifetime

## Getting Started

For this module you need to install R3 package, NuGetForUnity and ObservableCollections.

```
ObservableCollections can be installer vai NuGetForUnity

In Unity projects, you can installing ObservableCollections with NugetForUnity. 
If R3 integration is required, similarly install ObservableCollections.R3 via NuGetForUnity.
```

follow the instructions on home pages for these packages:

- https://github.com/Cysharp/R3
- https://github.com/GlitchEnzo/NuGetForUnity
- https://github.com/Cysharp/ObservableCollections

```

  "dependencies": {
    "com.unity.localization": "1.5.4",
    "com.unity.addressables": "2.6.0",
    "com.unigame.addressablestools" : "https://github.com/UnioGame/unigame.addressables",
    "com.unigame.unicore": "https://github.com/UnioGame/unigame.core.git",
    "com.unigame.localization": "https://github.com/UnioGame/unigame.localization.git",
    "com.unigame.rx": "https://github.com/UnioGame/unigame.rx.git",
    "com.cysharp.unitask" : "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
    "com.cysharp.r3": "https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity",
    "com.github-glitchenzo.nugetforunity": "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity"
  },

```


Add to your project manifiest by path [%UnityProject%]/Packages/manifiest.json new package:

```json
{
  "dependencies": {
    "com.unigame.viewsystem" : "https://github.com/UnioGame/unigame.viewsystem.git"
    "com.unigame.localization" : "https://github.com/UnioGame/unigame.localization.git",
    "com.cysharp.unitask" : "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask"
  }
}
```

## Getting Started

- Create View System Asset

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/view_asset_prefab.png)


## View System Settings

### Create Settings

```csharp
[MenuItem("Assets/UniGame/ViewSystem/Create ViewSystem")]
```

![](https://ibb.co/zVgHttfm)


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


### Custom Views Factory

View Factory - provide custom view creation logic. You can create your own factory by implementing:

 - **IViewFactory** 
 - **IViewFactoryProvider**

And select new provider in View System Settings

#### Enable Zenject DI Support

Add to your project scriptings define symbol "ZENJECT_ENABLED" to enable Zenject DI support

![image](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/viewfactoriessettings.png)

Anywhere in your initialization of game pass Zenject DiContainer to ZenjectViewFactoryProvider.Container static field

```csharp
public class ZenjectViewFactoryProvider : IViewFactoryProvider
{
    public static DiContainer Container { get; set; }
}
```

You can use Zenject DI module as an example to create your own custom DI support in view lines of code. Module is located in ZenjectViewModule directory

```csharp

//ZenjectViewFactory example
public class ZenjectViewFactory  : IViewFactory
{
    public ViewFactory _viewFactory;
    public DiContainer _container;
    
    public ZenjectViewFactory(DiContainer container,AsyncLazy readyStatus, IViewResourceProvider viewResourceProvider)
    {
        _container = container;
        _viewFactory = new ViewFactory(readyStatus, viewResourceProvider);
    }
    
    public async UniTask<IView> Create(string viewId, 
        string skinTag = "", 
        Transform parent = null, 
        string viewName = null,
        bool stayWorldPosition = false)
    {
        var view = await _viewFactory.Create(viewId, skinTag, parent, viewName, stayWorldPosition);
        if (view == null || view.GameObject == null) return view;
        var viewObject = view.GameObject;
        
        _container.InjectGameObject(viewObject);
        return view;
    }
}

```

## Pooling Support

## API References

### Views & ViewModels

The View System uses MVVM principles to manage UI with reactive data binding and automatic lifetime management.

#### View Lifetime Management

View System implements a sophisticated **two-level lifetime management** system that ensures proper cleanup of subscriptions and resources. This is critical for preventing memory leaks and managing complex UI hierarchies.

##### Two-Level LifeTime System

Each View maintains **two independent LifeTimes** that serve different purposes:

**ViewLifeTime** - Lifespan of the View Instance
- Created when View is instantiated
- Lives throughout entire View lifecycle (from creation to destruction)
- Manages resources that persist for the entire View lifecycle
- Terminated only when View is destroyed
- Used for: animations independent of model, component lifecycle events, long-lived resources

**ModelLifeTime** (aka `LifeTime`) - Lifespan of Data Subscriptions
- Created/restarted each time View is initialized with a model
- Manages all Observable subscriptions to model data
- **Automatically restarted** when model is changed
- Old subscriptions are automatically disconnected when restarted
- Used for: data binding, reactive streams, model-dependent operations

**Diagram: LifeTime Management Flow**

```
View Created
  |
  +--- ViewLifeTime.Start() ───────────────────────────────┐
        (Lives for entire View)                              |
                                                              |
  RegisterView(Model1)                                       |
  |
  +--- ModelLifeTime.Restart()                              |
        |                                                    |
        +--- Subscribe to Model1 data                        |
        |    (health, mana, etc.)                            |
        |                                                    |
        +--- Active Subscriptions ●●●                       |
                                                              |
  RegisterView(Model2)  ← Model Changed!                     |
        |                                                    |
        +--- ModelLifeTime.Restart()                         |
        |    (Old subscriptions auto-disconnected)           |
        |                                                    |
        +--- Subscribe to Model2 data                        |
        |    (fresh subscriptions)                           |
        |                                                    |
        +--- Active Subscriptions ●●●                       |
                                                              |
  Close()/Destroy()                                          |
  |                                                          |
  +--- ViewLifeTime.Terminate() ◄──────────────────────────┘
        ModelLifeTime.Terminate()
        Resources Released
        View Destroyed
```

###### Example: ViewLifeTime vs ModelLifeTime

```csharp
public class HealthBarView : ViewBase<CharacterViewModel>
{
    [SerializeField] private Image healthFill;
    
    protected override UniTask OnInitialize(CharacterViewModel model)
    {
        this.Bind(model.CurrentHealth, UpdateHealthBar);
        return UniTask.CompletedTask;
    }

}
```

##### ViewLifeTime vs ModelLifeTime

**Use ViewLifeTime for:**


**Use ModelLifeTime (LifeTime) for:**

- All data bindings to Observable fields
- Async operations triggered by model changes
- Model-dependent subscriptions
- React to data stream events

```csharp
// RECOMMENDED: Using Bind extensions
this.Bind(model.Health, UpdateDisplay);

// ALTERNATIVE: Direct Rx approach (what Bind does internally)
model.Health
    .Subscribe(UpdateDisplay)
    .AddTo(LifeTime);
```

**Key Difference:**
- ViewLifeTime: "Keep this while View exists"
- ModelLifeTime: "Keep this while this Model is active"

##### Automatic Subscription Cleanup

When a View is reinitialized with a new model, the system automatically cleans up old subscriptions:

```csharp
// Example: Character select screen with Bind extensions

public class CharacterDetailsView : ViewBase<CharacterViewModel>
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Slider healthSlider;
    
    protected override UniTask OnInitialize(CharacterViewModel model)
    {
        // RECOMMENDED: Using Bind extensions (auto-managed)
        this.Bind(model.Name, nameText)
            .Bind(model.Health, healthSlider);
        
        return UniTask.CompletedTask;
    }
}

// In controller code:
var character1ViewModel = new CharacterViewModel { Name = new("Hero") };
var character2ViewModel = new CharacterViewModel { Name = new("Villain") };

// User selects character 1
await detailsView.RegisterView(character1ViewModel);
// → OnInitialize() bound to character1 stats
// → Subscriptions connected to character1 data

// User selects character 2
await detailsView.RegisterView(character2ViewModel);
// → OnInitialize() called again
// → OLD subscriptions to character1 automatically → DISCONNECTED
// → NEW subscriptions to character2 → CREATED
// → No memory leak, no duplicate subscriptions
```

**Without LifeTime Management (Memory Leak):**

```csharp
// ❌ WRONG - This leaks memory!
public void BadExample(CharacterViewModel model)
{
    model.Health.Subscribe(x => UpdateUI(x));  // Never unsubscribes!
    // If you call this 100 times with different models,
    // you'll have 100 active subscriptions
}
```

**With LifeTime Management - Bind Extensions (RECOMMENDED):**

```csharp
// ✅ CORRECT - Using Bind (fluent API)
public void GoodExampleWithBind(CharacterViewModel model)
{
    this.Bind(model.Health, UpdateUI);
    // Auto-managed lifetime, disconnects on model change
}
```

##### View Status States

Views have distinct status states throughout their lifecycle:

| Status | Meaning | Next State |
|--------|---------|------------|
| **None** | Initial state | Shown, Hidden |
| **Shown** | View is visible and active | Hiding |
| **Showing** | Animation in progress | Shown |
| **Hidden** | View exists but not visible | Shown, Closed |
| **Hiding** | Hide animation in progress | Hidden |
| **Closed** | View destroyed, lifecycle ended | (final) |


**Complete Status Flow Diagram:**

```
RegisterView(model)
  ↓
Initialize(model)
  ↓
Status: None
  ↓
Show() called
  ↓
Status: Showing → OnShowAction() plays animation
  ↓
Status: Shown
  ↓
Hide() called
  ↓
Status: Hiding → OnHideAction() plays animation
  ↓
Status: Hidden
  ↓
Close() called
  ↓
Status: Closed → Destroy() → ViewLifeTime.Terminate()
```

**Observable Status Tracking:**

```csharp
// RECOMMENDED: Using Bind extensions
this.Bind(view.SelectStatus(ViewStatus.Hidden), 
    v => Debug.Log($"{v.SourceName} is hidden"));

// ALTERNATIVE: Direct Rx approach
view.Status
    .Where(status => status == ViewStatus.Shown)
    .Subscribe(_ => Debug.Log("View is now visible"))
    .AddTo(lifeTime);

// ALTERNATIVE: Using SelectStatus helper
view.SelectStatus(ViewStatus.Hidden)
    .Subscribe(v => Debug.Log($"{v.SourceName} is hidden"))
    .AddTo(lifeTime);
```

**Lifecycle Hooks by Status:**

```csharp
public class MyView : ViewBase<MyViewModel>
{
    [SerializeField] private Button closeButton;
    
    protected override UniTask OnInitialize(MyViewModel model)
    {
        // Called right after model attachment
        // Status: None → Shown (transitioning)
        
        // Bind button to close command
        this.Bind(closeButton, model.CloseCommand);
        
        // Or with action
        this.Bind(closeButton, Close);
        
        return UniTask.CompletedTask;
    }
    
    protected override UniTask OnShowAction()
    {
        // Called when transitioning to Shown
        // Use for entrance animations
        return PlayEntranceAnimation();
    }
    
    protected override UniTask OnHideAction()
    {
        // Called when transitioning to Hidden  
        // Use for exit animations
        return PlayExitAnimation();
    }
}
```


### Reactive Binding

All base Bind extensions use ViewModelLifetime that allows auto disconnect from data streams when ViewModel changed.

Binding extensions allow you to easily connect your view and data sources with a rich flow syntax and support Rx methods and async/await semantics.

#### Zero-Allocation Binding (Performance Optimization)

Bind extensions support **static lambda expressions** to eliminate closure allocations. This is especially important in performance-critical scenarios like frequent updates or animations.

**Static Lambda - Zero Allocation Closures:**

A `static` lambda cannot capture any local variables, which prevents the compiler from creating a display class for closure storage:

```csharp
// ✅ RECOMMENDED: Static lambda - zero allocation
// Compiler doesn't create display class for closure
this.Bind(model, purchaseStream, static (model, stream) => 
    stream.purchase.Execute(stream));

// ❌ Non-static lambda - closure allocation
// Compiler creates display class to capture variables
this.Bind(model, purchaseStream, (model, stream) => 
    stream.purchase.Execute(stream));
```

**When to use static lambdas:**
- Combining multiple observable streams
- High-frequency updates (animations, real-time data)
- Performance-critical UI sections
- The lambda doesn't need to capture `this` or local variables

**Example - Static Lambda Binding:**

```csharp
public class PurchaseView : ViewBase<PurchaseViewModel>
{
    [SerializeField] private Button purchaseButton;
    
    protected override UniTask OnInitialize(PurchaseViewModel model)
    {
        // Zero-allocation: static lambda combines two streams
        this.Bind(model, model.PurchaseStream, 
              static (purchaseData,viewModel) => viewModel.purchase.Execute(purchaseData));
        
        return UniTask.CompletedTask;
    }
}
```

**Static Lambda vs Direct Method Reference:**

```csharp
// ✅ Direct method reference - simple binding, zero allocation
this.Bind(model.Health, UpdateUI);

// ✅ Static lambda - complex binding with multiple parameters, zero allocation
this.Bind(model, itemStream, static (item,m) => m.ProcessItem(item));

// ❌ Non-static lambda - allocates closure class
this.Bind(model, itemStream, (item,m) => model.ProcessItemAndReport(item));  
// Captures 'model' in display class - not needed
```

**Static Lambda Requirements (C# 9+):**
- Cannot use `this` reference
- Cannot capture local variables
- Can only use method parameters and static members
- Compiler enforces these restrictions and prevents allocation


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
    public ReactiveProperty<string> label = new();
    public ReactiveProperty<string> value = new();
}

public TextMeshProUGUI label;
public TextMeshProUGUI value;

protected override UniTask OnViewInitialize(WindowViewModel model)
{
    this.Bind(model.label,label)
        .Bind(model.value,value);
    
    return UniTask.CompletedTask;
}

```

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

### Real Project Demo

![](https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/Editor/GitAssets/game-hud.gif)

## License

<a href="https://github.com/UniGameTeam/UniGame.ViewSystem/blob/master/LICENSE">MIT</a>
