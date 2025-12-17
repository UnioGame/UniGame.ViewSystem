# 🔌 Интеграция в существующие проекты

## Как заменить Odin Inspector в существующих скриптах

### Шаг 1: Обновите импорты

**Найдите и замените везде:**

```diff
- using Sirenix.OdinInspector;
+ using UniGame.ViewSystem.Inspector.Attributes;
```

### Шаг 2: Обновите Assembly Definitions

Если ваша сборка зависит от Odin Inspector, обновите ссылку:

**Было:**
```json
{
    "name": "YourAssembly",
    "references": [
        "Sirenix.OdinInspector.Attributes"
    ]
}
```

**Стало:**
```json
{
    "name": "YourAssembly",
    "references": [
        "unigame.viewsystem.inspector.runtime"
    ]
}
```

### Шаг 3: Замена атрибутов

Используйте Find & Replace (Ctrl+H) для замены:

#### BoxGroup - Одинаковый синтаксис ✅
```
Find:    \[BoxGroup\("([^"]+)"\)\]
Replace: [BoxGroup("$1")]
```

#### Title - Практически одинаковый
```csharp
// Было
[Title("Name")]
public string playerName;

// Стало (одинаково)
[Title("Name")]
public string playerName;

// Улучшено с подзаголовком
[Title("Name", "Player character name")]
public string playerName;
```

#### ReadOnly - Одинаковый синтаксис ✅
```
Find:    \[ReadOnly\]
Replace: [ReadOnly]
```

#### ShowIf/HideIf - Одинаковый синтаксис ✅
```
Find:    \[ShowIf\("([^"]+)"\)\]
Replace: [ShowIf("$1")]
```

#### Button - Практически одинаковый
```csharp
// Было (автоматическое имя)
[Button]
public void MyMethod() { }

// Стало (можно указать текст)
[Button("Custom Text")]
public void MyMethod() { }

// Или просто оставить
[Button]
public void MyMethod() { }
```

#### MinMaxSlider - Одинаковый синтаксис ✅
```csharp
// Было
[MinMaxSlider(0, 100)]
public Vector2 range = new Vector2(25, 75);

// Стало (одинаково)
[MinMaxSlider(0, 100)]
public Vector2 range = new Vector2(25, 75);
```

### Шаг 4: Удалите неподдерживаемые атрибуты

#### TabGroup
```csharp
// Было
[TabGroup("Tab1")]
public int field1;

[TabGroup("Tab2")]
public int field2;

// Стало - используйте BoxGroup с иерархией
[BoxGroup("Settings/General")]
public int field1;

[BoxGroup("Settings/Advanced")]
public int field2;
```

#### Range (Odin) → Range (Unity встроенный)
```csharp
// Было
[Range(0, 100)]
public float speed = 50;

// Стало - используйте встроенный Range
[Range(0, 100)]
public float speed = 50;
```

#### DropdownList
```csharp
// Было
[DropdownList("GetOptions")]
public string option;

private static List<string> GetOptions() => new() { "A", "B", "C" };

// Стало - просто SerializeField или выберите вариант:

// Вариант 1: Простое поле
[SerializeField]
public string option;

// Вариант 2: Используйте SerializeReference для полиморфизма
[SerializeReference]
public IOption option;
```

## Пример полной миграции

### До миграции (Odin)
```csharp
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Title("Core")]
    [BoxGroup("Core")]
    public string gameName = "MyGame";

    [BoxGroup("Core")]
    public int level = 1;

    [Title("Difficulty")]
    [BoxGroup("Gameplay")]
    [Range(0.5f, 2f)]
    public float difficultyMultiplier = 1f;

    [BoxGroup("Gameplay")]
    [ShowIf("useAdvanced")]
    public bool useAdvanced = false;

    [ShowIf("useAdvanced")]
    [BoxGroup("Advanced")]
    public float advancedParameter = 0.5f;

    [ReadOnly]
    [SerializeField]
    private int score = 0;

    [Button]
    public void StartGame()
    {
        Debug.Log("Game Started!");
    }

    [Button("Reset Score")]
    public void ResetScore()
    {
        score = 0;
    }
}
```

### После миграции (UI Toolkit)
```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Title("Core")]
    [BoxGroup("Core")]
    public string gameName = "MyGame";

    [BoxGroup("Core")]
    public int level = 1;

    [Title("Difficulty")]
    [BoxGroup("Gameplay")]
    [Range(0.5f, 2f)]  // Встроенный Unity Range
    public float difficultyMultiplier = 1f;

    [BoxGroup("Gameplay")]
    [ShowIf("useAdvanced")]
    public bool useAdvanced = false;

    [ShowIf("useAdvanced")]
    [BoxGroup("Advanced")]
    public float advancedParameter = 0.5f;

    [ReadOnly]
    [SerializeField]
    private int score = 0;

    [Button("Start Game")]
    public void StartGame()
    {
        Debug.Log("Game Started!");
    }

    [Button("Reset Score")]
    public void ResetScore()
    {
        score = 0;
    }
}
```

## Массовая замена через Find & Replace

### В VS Code / Rider

1. Откройте Find & Replace (Ctrl+H)
2. Включите Regular Expression (Alt+R)
3. Выполните замены по порядку:

```
# 1. Замена using
Find:    using Sirenix\.OdinInspector;
Replace: using UniGame.ViewSystem.Inspector.Attributes;

# 2. Title остаётся в покое (одинаковый синтаксис)

# 3. BoxGroup остаётся в покое (одинаковый синтаксис)

# 4. ReadOnly остаётся в покое (одинаковый синтаксис)

# 5. ShowIf остаётся в покое (одинаковый синтаксис)

# 6. Button остаётся в покое (совместимый синтаксис)

# 7. MinMaxSlider остаётся в покое (одинаковый синтаксис)
```

### Проблемные случаи для найти/заменить

#### [Range] - Нужна ручная проверка
```csharp
// Это Odin's Range (нужно заменить)
[Range(0, 100)]

// Это Unity's Range (оставить)
[UnityEngine.Range(0, 100)]

// Так что используйте встроенный Unity Range везде
```

#### [TabGroup] - Нужна ручная переработка
```csharp
// Вместо: [TabGroup("Group1")]
// Используйте: [BoxGroup("Group1")]
```

## Проверка после миграции

### Чек-лист

- [ ] Все `using Sirenix` удалены
- [ ] Все `using UniGame.ViewSystem.Inspector.Attributes` добавлены
- [ ] Assembly Definitions обновлены
- [ ] Нет ошибок компиляции
- [ ] Inspector отображает все поля
- [ ] ShowIf/HideIf работают
- [ ] Кнопки выполняют методы
- [ ] ReadOnly поля серые
- [ ] MinMaxSlider работает

### Команда для поиска остатков Odin

```bash
# Windows PowerShell
Get-ChildItem -Path . -Recurse -Include "*.cs" | 
  Select-String -Pattern "Sirenix|OdinInspector" | 
  Select-Object -ExpandProperty Path

# macOS/Linux
grep -r "Sirenix\|OdinInspector" --include="*.cs"
```

## Если что-то не работает

### Проблема: Атрибут не отображается в Inspector

**Решение:**
1. Проверьте импорт: `using UniGame.ViewSystem.Inspector.Attributes;`
2. Сохраните файл (Ctrl+S)
3. Вернитесь в Unity и дождитесь компиляции
4. Удалите Assembly-CSharp из Assets если есть
5. Пересоберите проект (Assets > Reimport All)

### Проблема: ShowIf/HideIf не работает

**Решение:**
1. Проверьте имя поля (case-sensitive)
2. Поле должно быть `bool` или возвращать `bool` из метода
3. Поле должно быть в том же классе

### Проблема: Кнопка не работает

**Решение:**
1. Метод должен быть `public` или `private`
2. Метод не должен иметь параметры
3. Метод должен быть `void`

## Интеграция с viewsystem пакетом

Если используете `com.unigame.viewsystem` для Views:

```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UniGame.ViewSystem;
using UnityEngine;

public class MyView : View
{
    [Title("View Configuration")]
    [BoxGroup("Settings")]
    [SerializeField] private string viewName;

    [BoxGroup("Settings")]
    [SerializeField] private bool closeOnEscape = true;

    [ReadOnly]
    [SerializeField] private bool isInitialized;

    [Button("Test View")]
    public void TestView()
    {
        Debug.Log("View test!");
    }
}
```

## Файлы для помощи

- **MIGRATION.md** - Полное руководство по миграции
- **BEST_PRACTICES.md** - Как писать хороший код
- **Examples/InspectorAttributesExample.cs** - Полный пример
- **README.md** - Справочник по атрибутам

## Поддержка

Если при миграции возникли проблемы:

1. Проверьте [MIGRATION.md](MIGRATION.md) - там подробно
2. Посмотрите [Examples/](Examples/) - рабочие примеры
3. Прочитайте [BEST_PRACTICES.md](BEST_PRACTICES.md) - может это проблема кода

**Готово! Теперь можете начинать использовать новый Inspector!** 🚀
