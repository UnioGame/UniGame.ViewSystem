# 📚 Полный индекс документации Inspector

Добро пожаловать в документацию библиотеки кастомных атрибутов UI Toolkit для Unity инспектора!

## 🚀 Начните отсюда

- **[QUICKSTART.md](QUICKSTART.md)** ⭐ - 5-минутный старт за несколько примеров
- **[README.md](README.md)** - Полная документация всех атрибутов и компонентов

## 📖 Полная документация

### Основное
| Файл | Описание |
|------|---------|
| [README.md](README.md) | Полная информация о пакете, структура, использование |
| [QUICKSTART.md](QUICKSTART.md) | Быстрый старт за 5 минут с примерами |
| [STRUCTURE.md](STRUCTURE.md) | Детальная структура папок и файлов |

### Миграция и совместимость
| Файл | Описание |
|------|---------|
| [MIGRATION.md](MIGRATION.md) | Пошаговая замена Odin Inspector на новые атрибуты |
| [INTEGRATION.md](INTEGRATION.md) | Интеграция в существующие проекты и массовая замена |
| [BEST_PRACTICES.md](BEST_PRACTICES.md) | Лучшие практики и примеры кода |

### Расширение
| Файл | Описание |
|------|---------|
| [EXTENDING.md](EXTENDING.md) | Как добавить новые атрибуты и PropertyDrawers |

### Служебная информация
| Файл | Описание |
|------|---------|
| [CHANGELOG.md](CHANGELOG.md) | История изменений и планы развития |
| [LICENSE](LICENSE) | MIT лицензия |

## 🔍 Быстрый поиск по задачам

### Я хочу...

#### ...начать использовать атрибуты
👉 [QUICKSTART.md](QUICKSTART.md) - Самый быстрый способ

#### ...узнать все доступные атрибуты
👉 [README.md](README.md#использование) - Полный список с примерами

#### ...перейти с Odin Inspector
👉 [MIGRATION.md](MIGRATION.md) - Полное руководство  
👉 [INTEGRATION.md](INTEGRATION.md) - Интеграция в существующий код

#### ...написать лучший код
👉 [BEST_PRACTICES.md](BEST_PRACTICES.md) - Советы и паттерны

#### ...добавить свой атрибут
👉 [EXTENDING.md](EXTENDING.md) - Подробное руководство

#### ...понять архитектуру пакета
👉 [STRUCTURE.md](STRUCTURE.md) - Полная структура

#### ...посмотреть примеры
👉 [Examples/InspectorAttributesExample.cs](Examples/InspectorAttributesExample.cs) - Рабочий пример

## 📚 Навигация по документам

### QUICKSTART.md - 5 минут
```
├─ Установка
├─ 4-шаговый быстрый старт
├─ Таблица основных атрибутов
├─ 3 готовых примера для копирования
├─ FAQ
└─ Дальнейшее обучение
```

### README.md - 15-20 минут
```
├─ Структура пакета
├─ Полный список атрибутов
│  ├─ BoxGroup
│  ├─ Title
│  ├─ ReadOnly
│  ├─ ShowIf / HideIf
│  ├─ MinMaxSlider
│  ├─ Button
│  ├─ HorizontalGroup
│  ├─ PropertySpace
│  └─ Tooltip
├─ Assembly Definitions
├─ Стили UI Toolkit
├─ Примеры использования
└─ Расширение библиотеки
```

### MIGRATION.md - 20-30 минут
```
├─ Обзор изменений
├─ Пошаговая миграция
│  ├─ Шаг 1: Удалить Odin
│  ├─ Шаг 2: Обновить Using
│  ├─ Шаг 3: Заменить атрибуты
│  └─ Шаг 4: Неподдерживаемые атрибуты
├─ 2 полных примера миграции
├─ Проверка после миграции
├─ Устранение проблем
└─ Performance Tips
```

### BEST_PRACTICES.md - 15-20 минут
```
├─ Организация кода
├─ Использование атрибутов правильно
├─ Структурирование классов
├─ Работа с вложенными типами
├─ Производительность
├─ Документирование
├─ Работа с условиями
├─ Тестирование в инспекторе
├─ Совместимость типов
└─ Резюме лучших практик
```

### EXTENDING.md - 20-25 минут
```
├─ Структура добавления атрибута
│  ├─ Шаг 1: Runtime атрибут
│  ├─ Шаг 2: Editor PropertyDrawer
│  └─ Шаг 3: CSS стили
├─ 2 примера реализации
├─ Использование утилит
├─ Тестирование
├─ Best Practices
└─ Common Pitfalls
```

## 🎯 Рекомендуемый порядок чтения

### Новичок
1. [QUICKSTART.md](QUICKSTART.md) - 5 минут
2. [Examples/InspectorAttributesExample.cs](Examples/InspectorAttributesExample.cs) - посмотреть код
3. [README.md](README.md) - 15 минут

### Опытный разработчик
1. [STRUCTURE.md](STRUCTURE.md) - понять архитектуру
2. [BEST_PRACTICES.md](BEST_PRACTICES.md) - лучшие подходы
3. [EXTENDING.md](EXTENDING.md) - расширение

### Миграция с Odin
1. [MIGRATION.md](MIGRATION.md) - полное руководство
2. [INTEGRATION.md](INTEGRATION.md) - интеграция в код
3. [BEST_PRACTICES.md](BEST_PRACTICES.md) - адаптация кода
4. [EXTENDING.md](EXTENDING.md) - пользовательские атрибуты

## 📝 Примеры в коде

### Простой пример
```csharp
using UniGame.ViewSystem.Inspector.Attributes;
using UnityEngine;

public class Simple : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField] private int value = 10;
    
    [Button]
    public void Click() => Debug.Log(value);
}
```

### Сложный пример
Смотрите: [Examples/InspectorAttributesExample.cs](Examples/InspectorAttributesExample.cs)

## 🛠️ Структура файлов

```
Inspector/
├── README.md              ← Начните отсюда для полной информации
├── QUICKSTART.md          ← 5-минутный старт
├── MIGRATION.md           ← Переход с Odin Inspector
├── BEST_PRACTICES.md      ← Лучшие практики кода
├── EXTENDING.md           ← Создание новых атрибутов
├── STRUCTURE.md           ← Архитектура пакета
├── CHANGELOG.md           ← История версий
├── LICENSE                ← MIT License
│
├── Runtime/               ← Runtime атрибуты
│   └── Attributes/        ← Все кастомные атрибуты
│
├── Editor/                ← Editor рендеры
│   ├── PropertyDrawers/   ← PropertyDrawers для атрибутов
│   ├── Utilities/         ← Вспомогательные утилиты
│   ├── Settings/          ← Конфигурация
│   └── Resources/Styles/  ← UI Toolkit стили
│
└── Examples/              ← Рабочие примеры
    └── InspectorAttributesExample.cs
```
## 🔗 Быстрые ссылки

- 🎯 [QUICKSTART.md](QUICKSTART.md) - Самый быстрый старт
- 📖 [README.md](README.md) - Полная документация
- 🔄 [MIGRATION.md](MIGRATION.md) - Замена Odin Inspector
- 🔌 [INTEGRATION.md](INTEGRATION.md) - Интеграция в проекты
- ✨ [BEST_PRACTICES.md](BEST_PRACTICES.md) - Хороший код
- 🔧 [EXTENDING.md](EXTENDING.md) - Свои атрибуты
- 📁 [STRUCTURE.md](STRUCTURE.md) - Структура проекта
- 📝 [Examples/](Examples/) - Рабочие примеры проекта
- 📝 [Examples/](Examples/) - Рабочие примеры

## 💡 Советы

- **Не знаете с чего начать?** → [QUICKSTART.md](QUICKSTART.md)
- **Нужен полный список атрибутов?** → [README.md](README.md)
- **Переходите с Odin?** → [MIGRATION.md](MIGRATION.md)
- **Хотите лучший код?** → [BEST_PRACTICES.md](BEST_PRACTICES.md)
- **Нужен свой атрибут?** → [EXTENDING.md](EXTENDING.md)

## 📞 Поддержка

Если у вас есть вопросы:
1. Проверьте [README.md](README.md)
2. Поищите в [BEST_PRACTICES.md](BEST_PRACTICES.md)
3. Посмотрите примеры в [Examples/](Examples/)
4. Прочитайте [MIGRATION.md](MIGRATION.md) если используете Odin

## 📅 Версия

- **Версия**: 2025.0.1
- **Лицензия**: MIT
- **Unity**: 2023.2+
- **Обновлено**: 2025-01-17

---

**Выберите документ выше и начните работу!** 🚀
