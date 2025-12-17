# Лучшие практики при использовании UI Toolkit атрибутов

## Организация кода

### ✅ Хорошо

```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Группируйте связанные поля
    [Title("Player Information")]
    [BoxGroup("Player")]
    [SerializeField] private string playerName = "Player";

    [BoxGroup("Player")]
    [SerializeField] private int level = 1;

    // Разделяйте секции空строками и заголовками
    [PropertySpace(10)]
    [Title("Combat Statistics")]
    [BoxGroup("Combat")]
    [SerializeField] private float attackPower = 10f;

    [BoxGroup("Combat")]
    [MinMaxSlider(0, 50)]
    [SerializeField] private Vector2 damageRange = new Vector2(10, 30);

    // ReadOnly для вычисляемых или системных полей
    [PropertySpace(10)]
    [ReadOnly]
    [SerializeField] private int experiencePoints = 0;
}
```

### ❌ Плохо

```csharp
using Sirenix.OdinInspector; // ❌ Старый импорт
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Все поля без организации
    [SerializeField] private string playerName = "Player";
    [SerializeField] private int level = 1;
    [SerializeField] private float attackPower = 10f;
    [SerializeField] private Vector2 damageRange = new Vector2(10, 30);
    [SerializeField] private int experiencePoints = 0;
    
    // Много других полей смешаны вместе...
}
```

## Использование атрибутов

### BoxGroup

```csharp
// ✅ Хорошо - ясные, краткие имена групп
[BoxGroup("Stats")]
public int health;

// ❌ Плохо - слишком длинные или неясные имена
[BoxGroup("Player_Statistics_And_Health_Settings")]
public int health;
```

### ShowIf / HideIf

```csharp
// ✅ Хорошо - логичные имена
[SerializeField] private bool useAdvancedSettings = false;

[ShowIf("useAdvancedSettings")]
[SerializeField] private float advancedParameter = 1f;

// ❌ Плохо - функция как условие
[ShowIf("IsAdvanced")] // ❌ Не будет работать с методом
[SerializeField] private float value;

private bool IsAdvanced() => useAdvancedSettings;
```

### Button

```csharp
// ✅ Хорошо - ясные действия
[Button("Reset Health")]
public void ResetHealth()
{
    currentHealth = maxHealth;
}

// ✅ Хорошо - параметр автоматическое имя из метода
[Button]
public void TakeDamage()
{
    // ...
}

// ❌ Плохо - метод с параметрами
[Button]
public void TakeDamage(int amount) // ❌ Кнопка не будет работать
{
    // ...
}
```

### Title

```csharp
// ✅ Хорошо - информативные заголовки
[Title("Player Configuration")]
[BoxGroup("Player")]
public string name;

[Title("Health Management", "Manage player health and damage")]
[BoxGroup("Health")]
public int currentHealth;

// ❌ Плохо - повторяющиеся или неясные заголовки
[Title("Data")]
public string name;
```

## Структурирование классов

### ✅ Рекомендуемый порядок

```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    // ===== Serialized Fields (Private) =====
    
    [Title("Configuration")]
    [SerializeField] private string configName;

    [BoxGroup("Settings")]
    [SerializeField] private int maxValue = 100;

    [BoxGroup("Settings")]
    [SerializeField] private float speed = 1f;

    // ===== Properties =====
    
    public int CurrentValue { get; private set; }
    
    public bool IsActive => enabled;

    // ===== Unity Lifecycle =====
    
    private void Awake()
    {
        // Initialization
    }

    private void Update()
    {
        // Main logic
    }

    // ===== Public Methods =====
    
    [Button]
    public void Reset()
    {
        CurrentValue = maxValue;
    }

    // ===== Private Methods =====
    
    private void InternalMethod()
    {
        // Helper method
    }
}
```

## Работа с вложенными типами

### ✅ Хорошо

```csharp
[System.Serializable]
public class PlayerStats
{
    [BoxGroup("Health")]
    [SerializeField] public int maxHealth = 100;

    [BoxGroup("Health")]
    [SerializeField] public int currentHealth = 100;

    [BoxGroup("Combat")]
    [SerializeField] public float attackPower = 10f;

    [BoxGroup("Combat")]
    [MinMaxSlider(0, 50)]
    [SerializeField] public Vector2 damageRange;
}

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
}
```

### ❌ Плохо

```csharp
// Смешивание атрибутов разных уровней
[BoxGroup("Stats")]
[SerializeField] private PlayerStats stats;

[SerializeField]
public class PlayerStats // ❌ Не должен быть public здесь
{
    public int health;
}
```

## Производительность

### ✅ Оптимизация

```csharp
// Кэшируйте результаты отражения
[SerializeField] private bool showAdvanced = false;

[ShowIf("showAdvanced")] // Только оценивается, когда нужно
[SerializeField] private float value;

// Используйте ReadOnly для уменьшения количества обновлений
[ReadOnly]
[SerializeField] private int cachedValue;
```

### ❌ Неоптимальность

```csharp
// Не используйте сложные условия в ShowIf
[ShowIf("ComplexConditionMethod")] // ❌ Вызывается часто
[SerializeField] private float value;

private bool ComplexConditionMethod()
{
    // Тяжёлые вычисления...
    return true;
}
```

## Документирование

### ✅ Хорошо документированный код

```csharp
/// <summary>
/// Manages player character statistics and progression
/// </summary>
public class PlayerCharacter : MonoBehaviour
{
    /// <summary>
    /// Maximum health value before level up
    /// </summary>
    [Title("Health Configuration")]
    [BoxGroup("Health")]
    [SerializeField] private int maxHealth = 100;

    /// <summary>
    /// Resets the player character to initial state
    /// </summary>
    [Button("Full Reset")]
    public void ResetCharacter()
    {
        // ...
    }
}
```

### ❌ Плохо - нет документации

```csharp
public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [Button]
    public void ResetCharacter()
    {
        // ...
    }
}
```

## Работа с условиями

### ✅ Правильно

```csharp
[SerializeField] private bool enableDamageDealing = true;

// Проверяйте существование поля
[ShowIf("enableDamageDealing")]
[SerializeField] private float damageAmount = 10f;

// Для инверсии используйте HideIf
[HideIf("enableDamageDealing")]
[SerializeField] private string disabledMessage = "Damage is disabled";
```

### ❌ Неправильно

```csharp
private bool EnableDamageDealing => true; // ❌ ShowIf работает только с полями

[ShowIf("EnableDamageDealing")] // ❌ Не найдёт свойство
[SerializeField] private float damageAmount = 10f;
```

## Тестирование в инспекторе

### Рекомендации

1. **Добавляйте кнопки для тестирования**
```csharp
[Button("Test Damage")]
public void TestDamage()
{
    TakeDamage(10);
    Debug.Log("Damage test: -10 HP");
}
```

2. **Используйте ReadOnly для значений, которые вы хотите наблюдать**
```csharp
[ReadOnly]
[SerializeField] private int currentHealth;
```

3. **Группируйте тестовые кнопки отдельно**
```csharp
[PropertySpace(15)]
[Title("Testing")]
[Button("Spawn Enemy")]
public void TestSpawnEnemy() { }

[Button("Trigger Event")]
public void TestTriggerEvent() { }
```

## Совместимость с разными типами

### ✅ Поддерживаемые типы

```csharp
// Примитивные типы
[SerializeField] private int intValue;
[SerializeField] private float floatValue;
[SerializeField] private string stringValue;
[SerializeField] private bool boolValue;

// Vector types
[SerializeField] private Vector2 vector2Value;
[SerializeField] private Vector3 vector3Value;
[SerializeField] private Vector4 vector4Value;

// Color
[SerializeField] private Color colorValue;

// Object references
[SerializeField] private GameObject gameObject;
[SerializeField] private Transform transform;
[SerializeField] private MonoBehaviour script;

// Arrays and Lists
[SerializeField] private int[] intArray;
[SerializeField] private List<int> intList;

// Serializable classes
[SerializeField] private CustomData data;
```

### ⚠️ Ограничения

```csharp
// ❌ Не будет работать с private свойствами без SerializeField
[Title("This won't show")]
private int myProperty { get; set; }

// ✅ Правильно
[Title("This will show")]
[SerializeField] private int myField;

public int MyProperty { get; private set; }
```

## Резюме лучших практик

| Практика | Статус | Пример |
|---|---|---|
| Группируйте поля с BoxGroup | ✅ | `[BoxGroup("Stats")]` |
| Используйте Title для секций | ✅ | `[Title("Configuration")]` |
| ReadOnly для вычисляемых полей | ✅ | `[ReadOnly] public int level;` |
| ShowIf для условного отображения | ✅ | `[ShowIf("enabled")]` |
| Кнопки для тестирования | ✅ | `[Button("Test")]` |
| Документируйте код | ✅ | `/// <summary>` |
| Используйте старые Odin импорты | ❌ | Удалите все `using Sirenix` |
| Кнопки с параметрами | ❌ | `[Button] public void Method(int x)` |
| Сложные условия в ShowIf | ❌ | Вызывают методы часто |
| Всё в одной группе | ❌ | Нет организации |

Следуйте этим практикам, и ваш код будет чистым, организованным и легко поддерживаемым! 🎯
