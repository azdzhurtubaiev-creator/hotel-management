# HotelManagement - система управління готелем

Навчальний проєкт з ООП (C#, .NET 9, MSTest). Предметна область - готель: номери, гості, працівники, бронювання та оплати.

## Структура

```
HotelManagement/
  Enums/        RoomType, BookingStatus, Position, PaymentMethod
  Interfaces/   IPerson, IPayable, IPrintable
  Delegates/    власні делегати (DiscountRule, BookingStatusHandler, PriceChangedHandler, HotelNotifier), правила знижок DiscountRules
  Models/       Person (abstract), Guest, Employee, Hotel, Room, Booking, Payment
  Program.cs    консольне меню
HotelManagement.Tests/   unit-тести (MSTest), 10 тест-класів
docs/                    діаграми класів (частини А, Б і С)
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

## Частина С
Делегати:
- власний делегат `DiscountRule` (правило знижки): `Booking.CalculateTotal(DiscountRule)`, `Booking.Pay(PaymentMethod, DiscountRule)`, `Hotel.CalculateExpectedIncome(DiscountRule)`; готові правила в класі `DiscountRules`;
- `Predicate<Room>` і `Predicate<Booking>`: `Hotel.FindRooms`, `Hotel.FindBookings`;
- `Func<Booking, decimal>`: `Hotel.SumBookings`;
- `Action<Room>` (у тому числі багатоадресний): `Hotel.ForEachRoom`.

Події (на основі власних делегатів):
- `Room.PriceChanged` (зміна ціни номера);
- `Booking.StatusChanged` (зміна статусу бронювання);
- `Hotel.Notify` (повідомлення готелю: додано номер, зареєстровано гостя, створено бронювання, пересилання двох попередніх подій); у `Program` на нього підписано два обробники, підписку можна вимкнути і ввімкнути в меню.

Додатково: бронювання запам'ятовує ціну номера на момент створення (`Booking.NightPrice`), тому зміна ціни номера не змінює вартість наявних бронювань.

Нові пункти меню: Номери - 5 (пошук за умовою, Predicate) і 6 (дії над номерами, Action); Бронювання - 8 (підсумки, Func і Predicate) і 9 (оплата зі знижкою, DiscountRule); Звіти - підменю з очікуваним доходом зі знижкою, журналом подій і вмиканням/вимиканням сповіщень. Діаграма класів: `docs/class-diagram-partC.png`. Усі unit-тести проходять (217 запусків).

## Запуск

```
dotnet run --project HotelManagement
dotnet test
```
