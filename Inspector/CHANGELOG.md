# Changelog

Все заметные изменения этого проекта будут документированы в этом файле.

## [2025.0.1] - 2025-01-17

### Added
- Первый релиз библиотеки кастомных атрибутов UI Toolkit
- Базовые атрибуты для инспектора:
  - `BoxGroupAttribute` - группировка полей в контейнер
  - `TitleAttribute` - добавление заголовка над полем
  - `ReadOnlyAttribute` - режим только-чтение для полей
  - `ShowIfAttribute` - условное отображение полей
  - `HideIfAttribute` - условное скрытие полей
  - `MinMaxSliderAttribute` - min/max слайдер для Vector2
  - `ButtonAttribute` - кнопка для вызова методов
  - `HorizontalGroupAttribute` - горизонтальное расположение полей
  - `PropertySpaceAttribute` - управление пространством между полями
  - `TooltipAttribute` - подсказки для полей
- Разделение на две сборки:
  - `unigame.viewsystem.inspector.runtime` - содержит атрибуты
  - `unigame.viewsystem.inspector.editor` - содержит PropertyDrawers
- UI Toolkit стили с относительными путями
- PropertyDrawer утилиты для работы с отражением
- InspectorUIHelper для создания UI элементов
- ButtonMethodDrawer для автоматического отображения кнопок методов
- Пример использования всех атрибутов (InspectorAttributesExample)
- Подробная документация на русском и английском

### Architecture
- Runtime часть не имеет зависимостей от Editor
- Editor часть использует только необходимые части UI Toolkit
- Все пути к ресурсам относительные для поддержки переноса пакета
- Настройка стилей через InspectorStylesConfig

## Migration Guide от Odin Inspector

Замены атрибутов:
```
OdinInspector          ->  UniGame Inspector
[BoxGroup("Group")]    ->  [BoxGroup("Group")]
[Title("Title")]       ->  [Title("Title")]
[ReadOnly]             ->  [ReadOnly]
[ShowIf("cond")]       ->  [ShowIf("cond")]
[Button]               ->  [Button]
[MinMaxSlider(0,100)]  ->  [MinMaxSlider(0, 100)]
```

## Будущие планы

- [ ] Группировка по вкладкам (TabGroup)
- [ ] Слайдер для диапазонов значений (SliderAttribute)
- [ ] Поле выбора из списка (DropdownAttribute)
- [ ] Кастомные цвета для групп
- [ ] Анимация развертывания/свертывания групп
- [ ] Сортировка полей по Order
- [ ] Поддержка SerializeReference
- [ ] Кастомные редакторы для сложных типов

## Requirements

- Unity 2023.2+
- UI Toolkit
- .NET Standard 2.1+

## License

MIT
