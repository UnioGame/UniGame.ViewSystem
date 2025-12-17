# Расширение библиотеки атрибутов

Этот документ описывает, как расширить библиотеку новыми атрибутами и PropertyDrawers.

## Структура добавления нового атрибута

### Шаг 1: Создание атрибута (Runtime)

Создайте новый файл в папке `Inspector/Runtime/Attributes/`:

```csharp
namespace UniGame.ViewSystem.Inspector.Attributes
{
    using System;
    
    /// <summary>
    /// Краткое описание атрибута
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class YourNewAttribute : InspectorAttributeBase
    {
        /// <summary>
        /// Основной параметр атрибута
        /// </summary>
        public string Parameter1 { get; }

        /// <summary>
        /// Опциональный параметр
        /// </summary>
        public bool Parameter2 { get; set; } = false;

        public YourNewAttribute(string parameter1)
        {
            Parameter1 = parameter1;
        }
    }
}
```

### Шаг 2: Создание PropertyDrawer (Editor)

Создайте новый файл в папке `Inspector/Editor/PropertyDrawers/`:

```csharp
namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Attributes;
    using Utilities;

    [CustomPropertyDrawer(typeof(YourNewAttribute))]
    public class YourNewPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attribute = (YourNewAttribute)this.attribute;
            var container = new VisualElement();

            // Здесь добавляйте вашу логику отрисовки

            var field = new PropertyField(property, property.displayName);
            container.Add(field);

            return container;
        }
    }
}
```

### Шаг 3: Добавление стилей (Optional)

Если вам нужны кастомные стили, добавьте их в `Inspector/Editor/Resources/Styles/InspectorStyles.uss`:

```css
.your-new-style {
    padding: 10px;
    margin: 5px;
    background-color: rgba(100, 100, 100, 0.2);
    border-radius: 4px;
}
```

## Примеры реализации

### Пример 1: Простой атрибут с одним параметром

```csharp
// Runtime
[AttributeUsage(AttributeTargets.Field)]
public class ColorLabelAttribute : InspectorAttributeBase
{
    public Color LabelColor { get; }

    public ColorLabelAttribute(string colorName = "white")
    {
        LabelColor = ParseColor(colorName);
    }

    private static Color ParseColor(string name)
    {
        return name.ToLower() switch
        {
            "red" => Color.red,
            "green" => Color.green,
            "blue" => Color.blue,
            _ => Color.white
        };
    }
}

// Editor
[CustomPropertyDrawer(typeof(ColorLabelAttribute))]
public class ColorLabelPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var attr = (ColorLabelAttribute)attribute;
        var container = new VisualElement();

        var label = new Label(property.displayName);
        label.style.color = attr.LabelColor;
        label.style.fontSize = 14;
        label.style.fontStyle = FontStyle.Bold;
        
        container.Add(label);
        container.Add(new PropertyField(property, ""));

        return container;
    }
}
```

### Пример 2: Атрибут с сложной логикой

```csharp
// Runtime
[AttributeUsage(AttributeTargets.Field)]
public class RangeWithResetAttribute : InspectorAttributeBase
{
    public float Min { get; }
    public float Max { get; }
    public float DefaultValue { get; set; }

    public RangeWithResetAttribute(float min, float max, float defaultValue)
    {
        Min = min;
        Max = max;
        DefaultValue = defaultValue;
    }
}

// Editor
[CustomPropertyDrawer(typeof(RangeWithResetAttribute))]
public class RangeWithResetPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var attr = (RangeWithResetAttribute)attribute;
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;

        var slider = new Slider(attr.Min, attr.Max)
        {
            label = property.displayName,
            value = property.floatValue,
            style =
            {
                flexGrow = 1,
                marginRight = 5
            }
        };

        slider.RegisterValueChangedCallback(evt =>
        {
            property.floatValue = evt.newValue;
            property.serializedObject.ApplyModifiedProperties();
        });

        var resetButton = new Button(() =>
        {
            property.floatValue = attr.DefaultValue;
            property.serializedObject.ApplyModifiedProperties();
            slider.value = attr.DefaultValue;
        })
        {
            text = "Reset",
            style =
            {
                width = 70
            }
        };

        container.Add(slider);
        container.Add(resetButton);

        return container;
    }
}
```

## Использование утилит

### PropertyDrawerUtility

```csharp
// Получить целевой объект
object target = PropertyDrawerUtility.GetTargetObjectOfProperty(property);

// Получить поле объекта
FieldInfo field = PropertyDrawerUtility.GetFieldFromObject(target, "fieldName");

// Получить значение условия
if (PropertyDrawerUtility.TryGetConditionValue(target, "isEnabled", out bool result))
{
    // result содержит значение
}

// Вызвать метод
MethodInfo method = PropertyDrawerUtility.GetMethodFromObject(target, "MethodName");
method?.Invoke(target, null);
```

### InspectorUIHelper

```csharp
// Создать контейнер группы
var groupContainer = InspectorUIHelper.CreateGroupContainer("MyGroup", showToggle: true, expanded: true);

// Создать элемент заголовка
var title = InspectorUIHelper.CreateTitleElement("My Title", "Subtitle", "#FF0000");

// Создать горизонтальную группу
var horizontalGroup = InspectorUIHelper.CreateHorizontalGroup();
```

## Тестирование атрибутов

1. Создайте тестовый скрипт в `Inspector/Examples/`:

```csharp
using UniGame.ViewSystem.Inspector;
using UnityEngine;

public class YourNewAttributeExample : MonoBehaviour
{
    [YourNewAttribute("parameter")]
    [SerializeField] private string testField;
}
```

2. Добавьте компонент на GameObject
3. Откройте Inspector и проверьте отрисовку

## Best Practices

1. **Всегда наследуйте от InspectorAttributeBase** для консистентности API
2. **Используйте правильные AttributeUsage** для указания целей атрибута
3. **Добавляйте XML документацию** для всех публичных членов
4. **Используйте CSS классы** вместо прямого установления стилей
5. **Кэшируйте вычисленные значения** для производительности
6. **Тестируйте с разными типами полей** (int, float, Vector3, и т.д.)
7. **Обрабатывайте null значения** правильно

## Common Pitfalls

### ❌ Неправильно
```csharp
// Изменение property без ApplyModifiedProperties
property.stringValue = newValue;

// Игнорирование null объектов
var field = PropertyDrawerUtility.GetFieldFromObject(target, "name");
var value = field.GetValue(target); // Может быть null!

// Жёсткие пути к ресурсам
var style = Resources.Load<StyleSheet>("Assets/Inspector/Styles/MyStyle");
```

### ✅ Правильно
```csharp
// Всегда применяйте модификации
property.stringValue = newValue;
property.serializedObject.ApplyModifiedProperties();

// Проверяйте null
FieldInfo field = PropertyDrawerUtility.GetFieldFromObject(target, "name");
if (field != null)
{
    var value = field.GetValue(target);
}

// Используйте относительные пути
var style = Resources.Load<StyleSheet>("Packages/com.unigame.viewsystem/Inspector/Editor/Resources/Styles/MyStyle");
```

## Версионирование

- Минорные изменения (новые атрибуты): увеличивать минорную версию
- Критические исправления: увеличивать patch версию
- Несовместимые изменения: увеличивать мажорную версию

Обновляйте CHANGELOG.md для каждого релиза!
