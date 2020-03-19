namespace UniGame.UiSystem.Runtime.Abstracts
{
    using UnityEngine;

    public interface IReadOnlyViewLayout 
    {
        // интерфейс называется ReadOnly, но при этом в нем есть методы явно меняющие состояние
        // лэйаута, наприер Close
        // Не меняющие состояния здесь только Get, Contains
        // из IViewLayout сюда так же можно перенести IViewStatus
        // и возможно ILifeTimeContext
        bool Contains(IView view);
        
        // Так же не понятен сценарий использования и добавляет еще один возможный путь управления
        void Hide<T>() where T :Component, IView;
        
        void HideAll();
        
        // Почему для CloseAll нет generic варианта?
        // сценарий использования также не понятен
        // Имхо методы *All<T>() не нужны. Потому что вызывающий хочет закрыть
        // все свои вьюшки подобного типа, то он вполне может хранить ссылки на все. Если же 
        // он хочет закрыть все вьюшки определенного типа, которые не обязательно он открывал
        // то можно очень много сломать
        void HideAll<T>() where T : Component, IView;

        // непонятен сценарий использования метода
        // загромождает интерфейс и создаёт несколько путей закрытия
        // для пользовательского кода хочется иметь один способ закрытия конретной вьюшки
        // а именно дерганье Close непосредственно у объетка
        void Close<T>() where T :Component, IView;

        TView Get<TView>() where TView : Component, IView;
        
        void CloseAll();

    }
}