# UniGame Inspector Attributes

Пакет кастомных атрибутов для инспектора, заменяющих Odin Inspector на UI Toolkit.

## Структура пакета

```
Inspector/
├── Runtime/                    # Runtime часть (атрибуты)
│   ├── Attributes/
│   │   ├── Abstract/
│   │   │   └── InspectorAttributeBase.cs
│   │   ├── BoxGroupAttribute.cs
│   │   ├── TitleAttribute.cs
│   │   ├── HorizontalGroupAttribute.cs
│   │   ├── PropertySpaceAttribute.cs
│   │   ├── ReadOnlyAttribute.cs
│   │   ├── TooltipAttribute.cs
│   │   ├── MinMaxSliderAttribute.cs
│   │   ├── ButtonAttribute.cs
│   │   ├── ShowIfAttribute.cs
│   │   └── HideIfAttribute.cs
│   └── unigame.viewsystem.inspector.runtime.asmdef
│
├── Editor/                     # Editor часть (рендеры и редакторы)
│   ├── PropertyDrawers/
│   │   ├── BoxGroupPropertyDrawer.cs
│   │   ├── TitlePropertyDrawer.cs
│   │   ├── ReadOnlyPropertyDrawer.cs
│   │   ├── ConditionalPropertyDrawer.cs
│   │   └── MinMaxSliderPropertyDrawer.cs
│   ├── Utilities/
│   │   ├── PropertyDrawerUtility.cs
│   │   ├── InspectorUIHelper.cs
│   │   └── ButtonMethodDrawer.cs
│   ├── Resources/
│   │   └── Styles/
│   │       └── InspectorStyles.uss
│   └── unigame.viewsystem.inspector.editor.asmdef
│
└── README.md
```

## Использование

### 1. BoxGroupAttribute - Группировка полей в коробку

```csharp
[BoxGroup("Settings")]
[SerializeField] private int maxHealth = 100;

[BoxGroup("Settings")]
[SerializeField] private int currentHealth = 50;
```

### 2. TitleAttribute - Заголовок над полем

```csharp
[Title("Player Configuration", "Основные параметры игрока")]
[SerializeField] private string playerName;

[Title("Damage Settings")]
[SerializeField] private float attackPower = 10f;
```

### 3. ReadOnlyAttribute - Только чтение

```csharp
[ReadOnly]
[SerializeField] private int level = 1;
```

### 4. ShowIfAttribute и HideIfAttribute - Условное отображение

```csharp
[SerializeField] private bool useAdvancedSettings = false;

[ShowIf("useAdvancedSettings")]
[SerializeField] private float advancedParameter = 0.5f;

[HideIf("useAdvancedSettings")]
[SerializeField] private float basicParameter = 1f;
```

### 5. MinMaxSliderAttribute - Min/Max слайдер для Vector2

```csharp
[MinMaxSlider(0, 100)]
[SerializeField] private Vector2 damageRange = new Vector2(10, 50);
```

### 6. ButtonAttribute - Кнопка для вызова метода

```csharp
[Button("Reset Health")]
public void ResetHealth()
{
    currentHealth = maxHealth;
}

[Button]
private void TestMethod()
{
    Debug.Log("Test button clicked!");
}
```

### 7. HorizontalGroupAttribute - Горизонтальное расположение полей

```csharp
[HorizontalGroup("Position")]
[SerializeField] private float x;

[HorizontalGroup("Position")]
[SerializeField] private float y;

[HorizontalGroup("Position")]
[SerializeField] private float z;
```

### 8. PropertySpaceAttribute - Пространство между полями

```csharp
[SerializeField] private int field1;

[PropertySpace(20)]
[SerializeField] private int field2;
```

## Assembly Definitions

Пакет разделён на две сборки:

### Runtime (`unigame.viewsystem.inspector.runtime.asmdef`)
- Содержит все атрибуты
- Нет зависимостей
- Может использоваться в runtime коде

### Editor (`unigame.viewsystem.inspector.editor.asmdef`)
- Содержит PropertyDrawers и утилиты для отрисовки
- Зависит от Runtime сборки
- Только для Editor

## Стили UI Toolkit

Стили хранятся в `Editor/Resources/Styles/InspectorStyles.uss` и используют относительные пути, поэтому пакет можно переносить без потери функциональности.

### Основные CSS классы:
- `.box-group-container` - контейнер группы
- `.box-group-foldout` - foldout для группы
- `.inspector-title` - стиль заголовка
- `.inspector-subtitle` - стиль подзаголовка
- `.horizontal-group` - горизонтальная группа
- `.inspector-button` - стиль кнопки

## Примеры

### Пример 1: Полная конфигурация персонажа

```csharp
using UniGame.ViewSystem.Inspector;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Title("Character Information")]
    [SerializeField] private string characterName;
    
    [BoxGroup("Stats")]
    [SerializeField] private int health = 100;
    
    [BoxGroup("Stats")]
    [SerializeField] private int mana = 50;
    
    [BoxGroup("Combat")]
    [MinMaxSlider(5, 20)]
    [SerializeField] private Vector2 damageRange = new Vector2(10, 15);
    
    [BoxGroup("Combat")]
    [SerializeField] private float attackSpeed = 1f;
    
    [SerializeField] private bool showAdvancedSettings = false;
    
    [ShowIf("showAdvancedSettings")]
    [BoxGroup("Advanced")]
    [SerializeField] private float criticalChance = 0.1f;
    
    [ReadOnly]
    [SerializeField] private int currentLevel = 1;
    
    [Button("Heal")]
    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, 100);
    }
    
    [Button("Reset")]
    public void Reset()
    {
        health = 100;
        mana = 50;
    }
}
```

### Пример 2: Использование утилит в Editor скрипте

```csharp
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UniGame.ViewSystem.Inspector.Editor.Utilities;

public class CustomInspector : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        
        // Добавить PropertyFields
        var serializedObject = new SerializedObject(target);
        root.Add(new PropertyField(serializedObject.FindProperty("characterName")));
        
        // Добавить кнопки методов с ButtonAttribute
        ButtonMethodDrawer.AddButtonMethodsToContainer(target, root);
        
        return root;
    }
}
```

## Расширение библиотеки

Чтобы добавить новый атрибут:

1. **Создайте атрибут в Runtime** (`Inspector/Runtime/Attributes/YourAttribute.cs`):
```csharp
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YourAttribute : InspectorAttributeBase
{
    // Ваша логика
}
```

2. **Создайте PropertyDrawer в Editor** (`Inspector/Editor/PropertyDrawers/YourPropertyDrawer.cs`):
```csharp
[CustomPropertyDrawer(typeof(YourAttribute))]
public class YourPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Ваша логика отрисовки
        return new PropertyField(property);
    }
}
```

## Миграция с Odin Inspector

Чтобы заменить Odin атрибуты:

| Odin Inspector | UniGame Inspector |
|---|---|
| `[BoxGroup("Group")]` | `[BoxGroup("Group")]` |
| `[Title("Title")]` | `[Title("Title")]` |
| `[ReadOnly]` | `[ReadOnly]` |
| `[ShowIf("condition")]` | `[ShowIf("condition")]` |
| `[Button]` | `[Button]` |
| `[MinMaxSlider(0, 100)]` | `[MinMaxSlider(0, 100)]` |

## Совместимость

- Unity 2023.2+
- Требует UI Toolkit
- Работает с SerializableObject

## Лицензия

MIT
