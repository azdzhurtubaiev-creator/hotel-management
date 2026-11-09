# HotelManagement - Проєкт (частина А)

Предметна область: **готель**. Готель має номери, наймає працівників, реєструє гостей і створює бронювання. Бронювання пов'язує гостя з номером на певні дати, може бути підтверджене, скасоване, оплачене, гість може заселитися і виселитися.

## Структура

```
HotelManagement/
  Enums/        RoomType, BookingStatus, Position, PaymentMethod
  Interfaces/   IPerson, IPayable, IPrintable
  Models/       Hotel, Room, Guest, Employee, Booking, Payment
HotelManagement.Tests/   unit-тести (MSTest)
docs/                    діаграми класів (етап 1 і етап 2)
```

## Відношення між класами

| Тип | Де |
|---|---|
| Реалізація | Guest, Employee → IPerson; Booking → IPayable, IPrintable; Room → IPrintable |
| Композиція | Hotel ◆ Room, Hotel ◆ Booking, Booking ◆ Payment |
| Агрегація | Hotel ◇ Employee, Hotel ◇ Guest |
| Асоціація | Booking → Guest, Booking → Room, Guest → Booking, класи → переліки |

## Стан

Частина А: каркас класів із заглушками `throw new NotImplementedException()` і повний набір unit-тестів (TDD).
Усі тести зараз мають статус **Failed** - реалізація буде в частині Б.
