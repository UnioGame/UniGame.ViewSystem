# Структура пакета Inspector

```
Inspector/
│
├── 📄 README.md                          # Основная документация
├── 📄 CHANGELOG.md                       # История изменений
├── 📄 MIGRATION.md                       # Руководство по миграции с Odin
├── 📄 EXTENDING.md                       # Как расширять библиотеку
├── 📄 BEST_PRACTICES.md                  # Лучшие практики
├── 📄 LICENSE                            # MIT License
│
├── 📁 Runtime/                           # RUNTIME СБОРКА (без Editor зависимостей)
│   ├── 📄 unigame.viewsystem.inspector.runtime.asmdef
│   │
│   └── 📁 Attributes/                    # Все кастомные атрибуты
│       ├── 📁 Abstract/
│       │   └── 📜 InspectorAttributeBase.cs      # Базовый класс для атрибутов
│       │
│       ├── 📜 BoxGroupAttribute.cs               # Группировка полей в контейнер
│       ├── 📜 TitleAttribute.cs                  # Заголовок над полем
│       ├── 📜 ReadOnlyAttribute.cs               # Поле только для чтения
│       ├── 📜 ShowIfAttribute.cs                 # Условное отображение
│       ├── 📜 HideIfAttribute.cs                 # Условное скрытие
│       ├── 📜 MinMaxSliderAttribute.cs           # Min/Max слайдер
│       ├── 📜 ButtonAttribute.cs                 # Кнопка для вызова метода
│       ├── 📜 HorizontalGroupAttribute.cs        # Горизонтальное расположение
│       ├── 📜 PropertySpaceAttribute.cs          # Пространство между полями
│       └── 📜 TooltipAttribute.cs                # Подсказка для поля
│
│
├── 📁 Editor/                            # EDITOR СБОРКА (зависит от Runtime)
│   ├── 📄 unigame.viewsystem.inspector.editor.asmdef
│   │
│   ├── 📁 PropertyDrawers/               # Рендеры для атрибутов
│   │   ├── 📜 BoxGroupPropertyDrawer.cs
│   │   ├── 📜 TitlePropertyDrawer.cs
│   │   ├── 📜 ReadOnlyPropertyDrawer.cs
│   │   ├── 📜 ConditionalPropertyDrawer.cs       # ShowIf + HideIf
│   │   └── 📜 MinMaxSliderPropertyDrawer.cs
│   │
│   ├── 📁 Utilities/                    # Вспомогательные утилиты
│   │   ├── 📜 PropertyDrawerUtility.cs           # Отражение и рефлексия
│   │   ├── 📜 InspectorUIHelper.cs               # Помощники UI Toolkit
│   │   └── 📜 ButtonMethodDrawer.cs              # Отрисовка кнопок методов
│   │
│   ├── 📁 Settings/                     # Конфигурация
│   │   └── 📜 InspectorStylesConfig.cs           # Загрузка стилей
│   │
│   └── 📁 Resources/
│       └── 📁 Styles/
│           └── 📄 InspectorStyles.uss            # UI Toolkit стили
│
│
└── 📁 Examples/                          # Примеры использования
    └── 📜 InspectorAttributesExample.cs           # Полный пример всех атрибутов
```

## Быстрый старт

### 1. Использование атрибутов

```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class MyScript : MonoBehaviour
{
    [Title("Settings")]
    [BoxGroup("Basic")]
    [SerializeField] private string name = "Test";
    
    [BoxGroup("Basic")]
    [ShowIf("enableAdvanced")]
    [SerializeField] private float value = 1f;
    
    [SerializeField] private bool enableAdvanced = false;
    
    [Button("Execute")]
    public void Execute()
    {
        Debug.Log($"Name: {name}, Value: {value}");
    }
}
```

### 2. Создание нового атрибута

```csharp
// Runtime: Inspector/Runtime/Attributes/MyAttribute.cs
[AttributeUsage(AttributeTargets.Field)]
public class MyAttribute : InspectorAttributeBase { }

// Editor: Inspector/Editor/PropertyDrawers/MyPropertyDrawer.cs
[CustomPropertyDrawer(typeof(MyAttribute))]
public class MyPropertyDrawer : PropertyDrawer { }
```

## Архитектурные решения

### ✅ Разделение на Runtime и Editor

**Runtime** (`unigame.viewsystem.inspector.runtime`)
- Содержит только атрибуты (простые классы без зависимостей)
- Может использоваться в runtime коде
- Размер: ~1KB
- Нет Editor зависимостей

**Editor** (`unigame.viewsystem.inspector.editor`)
- Содержит PropertyDrawers и утилиты
- Только для Editor использования
- Зависит от Runtime
- Размер: ~50KB

### ✅ Относительные пути для стилей

```csharp
// Правильно - работает при перемещении пакета
const string INSPECTOR_PACKAGE_PATH = "Packages/com.unigame.viewsystem/Inspector";

// Неправильно - ломается при перемещении
const string OLD_PATH = "Assets/Plugins/Inspector";
```

## Интеграция с viewsystem пакетом

Библиотека `Inspector` является частью `com.unigame.viewsystem` пакета и:

1. **Не зависит** от других частей viewsystem
2. **Может использоваться** независимо
3. **Полностью опциональна** (можно использовать встроенные атрибуты Unity)
4. **Улучшает** разработку с viewsystem компонентами

## Performance

- **Атрибуты**: 0 overhead в runtime (только metadata)
- **PropertyDrawers**: Загружаются только в Editor
- **Стили**: Кэшируются после первой загрузки
- **Отражение**: Используется только при отрисовке инспектора

## Лицензия

MIT - свободен для личного и коммерческого использования

## Поддержка

- 📖 Документация: README.md, MIGRATION.md, EXTENDING.md, BEST_PRACTICES.md
- 🔍 Примеры: Examples/InspectorAttributesExample.cs
- 🐛 Ошибки: Проверьте Console в Unity Editor
- 💡 Идеи расширения: EXTENDING.md
