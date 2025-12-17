# Миграция с Odin Inspector на UI Toolkit

Этот документ описывает процесс замены Odin Inspector на встроенные кастомные атрибуты UI Toolkit в пакете `com.unigame.viewsystem`.

## Обзор изменений

### Преимущества миграции

✅ **Удаление внешней зависимости** - больше не требуется лицензия Odin Inspector  
✅ **Меньше размер пакета** - кастомные атрибуты занимают намного меньше места  
✅ **Лучшая интеграция с UI Toolkit** - встроенная поддержка современного UI Framework'а  
✅ **Полная контроль** - можно легко расширять и модифицировать  
✅ **Поддержка относительных путей** - пакет работает в любой папке  

### Что было изменено

| Компонент | До (Odin) | После (UI Toolkit) |
|---|---|---|
| Инспектор | Собственный движок отрисовки | UI Toolkit PropertyDrawers |
| Атрибуты | `Sirenix.OdinInspector` | `UniGame.ViewSystem.Inspector.Attributes` |
| Размер пакета | ~10MB | ~1MB |
| Зависимости | Odin Inspector | Нет (кроме UI Toolkit) |
| Стили | Встроенные в Odin | Кастомные USS файлы |

## Пошаговая миграция проекта

### Шаг 1: Удаление Odin Inspector

```bash
# Удалить Odin Inspector из package.json
# или использовать Package Manager в Unity
```

### Шаг 2: Обновление Using statements

Замените все Odin import'ы:

```csharp
// До
using Sirenix.OdinInspector;

// После
using UniGame.ViewSystem.Inspector.Attributes;
```

### Шаг 3: Замена атрибутов

#### BoxGroup

```csharp
// До
[BoxGroup("Settings")]
public int maxHealth = 100;

// После
[BoxGroup("Settings")]
public int maxHealth = 100;
```

#### Title

```csharp
// До
[Title("Player Configuration")]
public string playerName;

// После
[Title("Player Configuration", "Description")]
public string playerName;
```

#### ReadOnly

```csharp
// До
[ReadOnly]
public int level = 1;

// После
[ReadOnly]
public int level = 1;
```

#### ShowIf / HideIf

```csharp
// До
[ShowIf("useAdvanced")]
public float advancedParameter = 1f;

// После
[ShowIf("useAdvanced")]
public float advancedParameter = 1f;
```

#### Button

```csharp
// До
[Button]
public void ResetStats()
{
    // ...
}

// После
[Button("Reset Stats")]
public void ResetStats()
{
    // ...
}
```

#### MinMaxSlider

```csharp
// До
[MinMaxSlider(0, 100)]
public Vector2 damageRange = new Vector2(10, 50);

// После
[MinMaxSlider(0, 100)]
public Vector2 damageRange = new Vector2(10, 50);
```

### Шаг 4: Удаление неподдерживаемых атрибутов

Некоторые Odin атрибуты не имеют прямых аналогов. Вот как их заменить:

#### Odin: `[TabGroup]` → UI Toolkit: Используйте `[BoxGroup]` с разными группами

```csharp
// До (Odin)
[TabGroup("Tab1")]
public int field1;

[TabGroup("Tab2")]
public int field2;

// После (UI Toolkit) - пока используйте BoxGroup
[BoxGroup("Settings/General")]
public int field1;

[BoxGroup("Settings/Advanced")]
public int field2;
```

#### Odin: `[Range]` → Unity: Встроенный `[Range]`

```csharp
// До (Odin)
[Range(0, 100)]
public float speed = 50;

// После (Unity встроенный)
[Range(0, 100)]
public float speed = 50;
```

#### Odin: `[DropdownList]` → UI Toolkit: Используйте `[SerializeReference]`

```csharp
// До (Odin)
[DropdownList("GetOptions")]
public string option;

// После (обычное поле или SerializeReference)
[SerializeField]
public string option;
```

#### Odin: `[InlineProperty]` → Обычные поля

```csharp
// До (Odin)
[InlineProperty]
public CustomData data;

// После
[SerializeField]
public CustomData data;
```

## Примеры миграции

### Пример 1: Простой класс с настройками

**До:**
```csharp
using Sirenix.OdinInspector;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [Title("Player")]
    [BoxGroup("Player")]
    [MinMaxSlider(1, 100)]
    public Vector2 healthRange = new Vector2(50, 100);

    [BoxGroup("Player")]
    [ReadOnly]
    public int level = 1;

    [Title("Gameplay")]
    [BoxGroup("Gameplay")]
    public float gameSpeed = 1f;

    [BoxGroup("Gameplay")]
    [ShowIf("gameSpeed", 1)]
    public bool normalSpeed;

    [Button("Apply Settings")]
    public void ApplySettings()
    {
        Debug.Log("Settings applied!");
    }
}
```

**После:**
```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [Title("Player Configuration")]
    [BoxGroup("Player")]
    [MinMaxSlider(1, 100)]
    public Vector2 healthRange = new Vector2(50, 100);

    [BoxGroup("Player")]
    [ReadOnly]
    public int level = 1;

    [Title("Gameplay Settings")]
    [BoxGroup("Gameplay")]
    public float gameSpeed = 1f;

    [BoxGroup("Gameplay")]
    [ShowIf("gameSpeed")]
    public bool normalSpeed;

    [Button("Apply Settings")]
    public void ApplySettings()
    {
        Debug.Log("Settings applied!");
    }
}
```

### Пример 2: Сложный класс с условным отображением

**До:**
```csharp
using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Title("Basic Info")]
    public string characterName;
    public int level = 1;

    [FoldoutGroup("Stats")]
    [BoxGroup("Stats/Health")]
    public int maxHealth = 100;

    [BoxGroup("Stats/Health")]
    public int currentHealth = 100;

    [FoldoutGroup("Advanced")]
    [ShowIf("enableAdvanced")]
    public bool enableAdvanced = false;

    [ShowIf("enableAdvanced")]
    [FoldoutGroup("Advanced")]
    [BoxGroup("Advanced/Combat")]
    public float criticalChance = 0.1f;

    [Button]
    public void Heal()
    {
        currentHealth = maxHealth;
    }
}
```

**После:**
```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Title("Basic Information")]
    public string characterName;
    public int level = 1;

    [BoxGroup("Stats")]
    public int maxHealth = 100;

    [BoxGroup("Stats")]
    public int currentHealth = 100;

    [PropertySpace(15)]
    public bool enableAdvanced = false;

    [ShowIf("enableAdvanced")]
    [BoxGroup("Combat")]
    public float criticalChance = 0.1f;

    [Button("Heal")]
    public void Heal()
    {
        currentHealth = maxHealth;
    }
}
```

## Проверка после миграции

### Чек-лист

- [ ] Все `using Sirenix` удалены
- [ ] Все атрибуты заменены на `UniGame.ViewSystem.Inspector`
- [ ] Нет ошибок компиляции
- [ ] Assembly Definition ссылаются на правильные сборки
- [ ] Инспектор отображает поля правильно
- [ ] Кнопки работают корректно
- [ ] Условные поля (ShowIf/HideIf) работают
- [ ] Стили применяются корректно

### Тестирование

1. Откройте сцену с компонентами, использующими новые атрибуты
2. Проверьте, что инспектор отображает все поля
3. Протестируйте ShowIf/HideIf условия
4. Нажмите кнопки и проверьте вызовы методов
5. Отредактируйте значения и убедитесь, что сохраняются

## Устранение проблем

### Проблема: Поле не отображается в инспекторе

**Решение:** Убедитесь, что:
1. Поле имеет атрибут `[SerializeField]` (для private)
2. Тип поля сериализуемый (int, float, string, Vector3, и т.д.)
3. Assembly Definition правильно настроена

### Проблема: ShowIf/HideIf не работает

**Решение:** Проверьте:
1. Имя условного поля точное (case-sensitive)
2. Условное поле существует и является bool
3. Условное поле в том же классе

```csharp
[SerializeField] private bool useAdvanced = false;

[ShowIf("useAdvanced")] // ✅ Правильно
[SerializeField] private float value;

// ❌ Неправильно:
// [ShowIf("UseAdvanced")] - неверная case
// [ShowIf("_useAdvanced")] - неверное имя
```

### Проблема: Кнопка не вызывает метод

**Решение:** Убедитесь:
1. Метод является `public` или `private`
2. Метод не имеет параметров
3. Возвращаемый тип - `void`

```csharp
[Button("Click me")]
public void MyMethod() // ✅ Правильно
{
    Debug.Log("Clicked!");
}

// ❌ Неправильно:
// private int MyMethod() { return 1; } - имеет return value
// public void MyMethod(int param) { } - имеет параметры
```

## Performance Tips

1. Используйте `[ReadOnly]` для полей, которые не должны редактироваться
2. Группируйте связанные поля с `[BoxGroup]` для лучшей организации
3. Используйте `[ShowIf]` для скрытия не нужных полей, чтобы улучшить читаемость

## Дополнительные ресурсы

- 📖 [README.md](README.md) - Полная документация
- 🔧 [EXTENDING.md](EXTENDING.md) - Как расширить библиотеку
- 📝 [Examples/InspectorAttributesExample.cs](Examples/InspectorAttributesExample.cs) - Примеры кода

## Поддержка

Если вы столкнулись с проблемами при миграции:

1. Проверьте логи Console в Unity
2. Убедитесь, что все Assembly Definition настроены правильно
3. Очистите Library папку и пересобрите проект (`Assets > Reimport All`)
4. Проверьте, что используется правильный namespace

## Заключение

Миграция на встроенные UI Toolkit атрибуты даёт вам:
- Полный контроль над функциональностью
- Меньший размер проекта
- Лучшую интеграцию с Unity
- Возможность легко расширять под свои нужды

Удачи с миграцией! 🚀
