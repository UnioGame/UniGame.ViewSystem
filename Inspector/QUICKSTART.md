# Установка и быстрый старт

## Установка

### Опция 1: Уже входит в пакет `com.unigame.viewsystem`

Если вы используете пакет `com.unigame.viewsystem` версии 2025.0.1+, то Inspector библиотека уже включена.

### Опция 2: Отдельно (если нужно)

Папка `Inspector` находится в:
```
Packages/com.unigame.viewsystem/Inspector/
```

## Быстрый старт за 5 минут

### 1️⃣ Создайте простой скрипт

```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    [Title("Difficulty Settings")]
    [BoxGroup("Difficulty")]
    [SerializeField] private int enemyCount = 5;

    [BoxGroup("Difficulty")]
    [MinMaxSlider(0.5f, 2f)]
    [SerializeField] private Vector2 difficultyMultiplier = new Vector2(1f, 1.5f);

    [PropertySpace(10)]
    [ReadOnly]
    [SerializeField] private float gameStartTime;

    [Button("Start Game")]
    public void StartGame()
    {
        gameStartTime = Time.time;
        Debug.Log($"Game started with {enemyCount} enemies!");
    }

    [Button("Reset")]
    public void Reset()
    {
        gameStartTime = 0;
    }
}
```

### 2️⃣ Добавьте скрипт на GameObject

1. Создайте пустой GameObject или используйте существующий
2. Добавьте скрипт компонентом (Drag & Drop или Add Component)

### 3️⃣ Откройте Inspector

Вы должны увидеть:
- 🏷️ Заголовок "Difficulty Settings"
- 📦 Контейнер группы "Difficulty" с двумя полями
- 📊 Min/Max слайдер для difficultyMultiplier
- 🔒 Серое поле gameStartTime (только чтение)
- 🔘 Две кнопки: "Start Game" и "Reset"

### 4️⃣ Протестируйте

- Измените значения полей
- Нажимите кнопки
- Посмотрите логи в Console

## Основные атрибуты

| Атрибут | Назначение | Пример |
|---------|-----------|--------|
| `[Title("...")]` | Заголовок над полем | `[Title("Health")]` |
| `[BoxGroup("...")]` | Группировка полей | `[BoxGroup("Stats")]` |
| `[ReadOnly]` | Только чтение | `[ReadOnly] public int level;` |
| `[ShowIf("...")]` | Условное отображение | `[ShowIf("enabled")]` |
| `[Button("...")]` | Кнопка метода | `[Button("Reset")] public void Reset()` |
| `[MinMaxSlider(min, max)]` | Диапазон значений | `[MinMaxSlider(0, 100)]` |

## Примеры для копирования

### Пример 1: Конфигурация игрока

```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class PlayerConfig : MonoBehaviour
{
    [Title("Character")]
    [BoxGroup("Player")]
    public string characterName = "Hero";

    [BoxGroup("Player")]
    public int level = 1;

    [PropertySpace(10)]
    [Title("Combat")]
    [BoxGroup("Combat")]
    [MinMaxSlider(5, 50)]
    public Vector2 damageRange = new Vector2(10, 30);

    [BoxGroup("Combat")]
    public float attackSpeed = 1f;

    [Button("Level Up")]
    public void LevelUp()
    {
        level++;
        Debug.Log($"Now level {level}!");
    }
}
```

### Пример 2: С условным отображением

```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class GameModes : MonoBehaviour
{
    [SerializeField] private bool useAdvancedMode = false;

    [ShowIf("useAdvancedMode")]
    [Title("Advanced Settings")]
    [BoxGroup("Advanced")]
    public float parameter1 = 0.5f;

    [ShowIf("useAdvancedMode")]
    [BoxGroup("Advanced")]
    public float parameter2 = 1f;

    [HideIf("useAdvancedMode")]
    [Title("Basic Mode")]
    public float simpleValue = 1f;

    [Button("Save Config")]
    public void SaveConfig()
    {
        Debug.Log("Configuration saved!");
    }
}
```

### Пример 3: Управление состоянием

```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [ReadOnly]
    public bool isInitialized;

    [ReadOnly]
    public float elapsedTime;

    [Title("Controls")]
    [Button("Initialize", 40)]
    public void Initialize()
    {
        isInitialized = true;
        elapsedTime = 0;
        Debug.Log("Initialized!");
    }

    [Button("Reset", 40)]
    public void Reset()
    {
        isInitialized = false;
        elapsedTime = 0;
        Debug.Log("Reset!");
    }

    private void Update()
    {
        if (isInitialized)
            elapsedTime += Time.deltaTime;
    }
}
```

## Часто задаваемые вопросы

### ❓ Почему мой атрибут не работает?

Проверьте:
1. Импорт: `using UniGame.ViewSystem.Inspector.Attributes;`
2. Поле с `[SerializeField]` (для private)
3. Сохраните скрипт (Ctrl+S)
4. Вернитесь в Unity (дождитесь компиляции)

### ❓ Как скрыть поле условно?

Используйте `[HideIf("fieldName")]` вместо `[ShowIf]`:

```csharp
[SerializeField] private bool showExpert = false;

[HideIf("showExpert")] // Скрыто, когда showExpert = true
[SerializeField] private float value;
```

### ❓ Как добавить кнопку?

```csharp
[Button("Click Me")] // Можно указать текст
public void MyMethod()
{
    // Код вызывается при клике
}

[Button] // Или просто [Button] - использует имя метода
public void Test()
{
}
```

### ❓ Метод с параметрами в кнопке?

```csharp
// ❌ Не работает
[Button]
public void TakeDamage(int amount) { }

// ✅ Используйте параметр по умолчанию
[SerializeField] private int damageAmount = 10;

[Button("Test Damage")]
public void TestDamage()
{
    TakeDamage(damageAmount);
}
```

## Дальнейшее обучение

- 📖 **README.md** - Полная документация всех атрибутов
- 🔧 **EXTENDING.md** - Как создавать свои атрибуты
- 📋 **MIGRATION.md** - Миграция с Odin Inspector
- ✨ **BEST_PRACTICES.md** - Лучшие практики
- 📦 **Examples/** - Рабочие примеры кода

## Совместимость

✅ Unity 2023.2+  
✅ .NET Standard 2.1+  
✅ Windows, macOS, Linux  
✅ Все платформы (Inspector работает в Editor)

## Поддержка

Если возникнут вопросы:
1. Проверьте [README.md](README.md)
2. Посмотрите на [Examples/](Examples/)
3. Прочитайте [MIGRATION.md](MIGRATION.md) если переходите с Odin

## Следующие шаги

- [ ] Проверить все атрибуты в примере
- [ ] Выбрать нужные атрибуты для проекта
- [ ] Заменить Odin Inspector на новые атрибуты
- [ ] Прочитать BEST_PRACTICES.md для хорошего кода
- [ ] Расширить библиотеку своими атрибутами если нужно

🎉 Готово! Теперь можете использовать InspectorAttributes в своих проектах!
