# HotelManagement - система управління готелем

Навчальний проєкт з ООП (C#, .NET 9, MSTest). Предметна область - готель: номери, гості, працівники, бронювання та оплати.

## Структура

```
HotelManagement/
  Enums/        RoomType, BookingStatus, Position, PaymentMethod
  Interfaces/   IPerson, IPayable, IPrintable
  Models/       Person (abstract), Guest, Employee, Hotel, Room, Booking, Payment
  Program.cs    консольне меню
HotelManagement.Tests/   unit-тести (MSTest), 8 тест-класів
docs/                    діаграми класів
```

## Частина А
Каркас класів із заглушками та unit-тести за TDD (діаграми `docs/class-diagram-stage1.png`, `docs/class-diagram-stage2.png`).

## Частина Б
- абстрактний клас `Person` (абстрактна властивість `Role`, абстрактний метод `GetGreeting`, віртуальні `GetInfo` і `GetFullName`), від нього успадковані `Guest` і `Employee`;
- інтерфейси .NET: `IComparable<Person>`, `IComparable<Room>` (сортування), `IEnumerable<Room>` (foreach по готелю);
- множинна реалізація інтерфейсів: `Person : IPerson, IPrintable, IComparable<Person>`, `Room : IPrintable, IComparable<Room>`, `Booking : IPayable, IPrintable`;
- поліморфізм: перевизначення віртуального `GetInfo` у нащадках і виклик через змінні типу `Person`, `IPrintable`, `IPayable`;
- заглушки замінено реалізацією, усі unit-тести проходять;
- консольне меню: номери, гості, працівники, бронювання, пошук вільних номерів, звіти, демонстрація поліморфізму, демо-дані.

Діаграма класів: `docs/class-diagram-partB.png`.

## Запуск

```
dotnet run --project HotelManagement
dotnet test
```
