using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HotelManagement
{
    public class Program
    {
        private const string DateFormat = "dd.MM.yyyy";

        private static Hotel _hotel = new Hotel("Харків Палас", 4, "Харків, вул. Сумська, 10");

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.WriteLine($"=== Система управління готелем «{_hotel.Name}» ({_hotel.Stars}*) ===");
            Console.WriteLine($"Адреса: {_hotel.Address}");

            bool running = true;

            while (running)
            {
                ShowMenu("ГОЛОВНЕ МЕНЮ",
                    "1. Номери",
                    "2. Гості",
                    "3. Працівники",
                    "4. Бронювання",
                    "5. Пошук вільних номерів",
                    "6. Звіти",
                    "7. Демонстрація поліморфізму",
                    "8. Заповнити демо-даними",
                    "0. Вихід");

                string choice = ReadChoice();

                try
                {
                    switch (choice)
                    {
                        case "1": RoomsMenu(); break;
                        case "2": GuestsMenu(); break;
                        case "3": EmployeesMenu(); break;
                        case "4": BookingsMenu(); break;
                        case "5": SearchFreeRooms(); break;
                        case "6": ShowReports(); break;
                        case "7": DemonstratePolymorphism(); break;
                        case "8": FillDemoData(); break;
                        case "0": running = false; break;
                        default: Console.WriteLine("Невірний пункт меню!"); break;
                    }
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                }
            }

            Console.WriteLine("Роботу програми завершено.");
        }

        private static void RoomsMenu()
        {
            ShowMenu("НОМЕРИ",
                "1. Показати всі номери",
                "2. Додати номер",
                "3. Змінити ціну номера",
                "4. Видалити номер",
                "0. Назад");

            switch (ReadChoice())
            {
                case "1": ShowRooms(); break;
                case "2": AddRoom(); break;
                case "3": ChangeRoomPrice(); break;
                case "4": RemoveRoom(); break;
                case "0": break;
                default: Console.WriteLine("Невірний пункт меню!"); break;
            }
        }

        private static void ShowRooms()
        {
            if (_hotel.Rooms.Count == 0)
            {
                Console.WriteLine("Номерів ще немає.");
                return;
            }

            Console.WriteLine("Номери (відсортовано за номером, IComparable<Room>):");

            foreach (Room room in _hotel.GetSortedRooms())
            {
                Console.WriteLine("  " + room.GetInfo());
            }
        }

        private static void AddRoom()
        {
            int number = ReadInt("Номер кімнати: ");
            RoomType type = ReadRoomType();
            decimal price = ReadDecimal("Ціна за ніч, грн: ");

            Room room = _hotel.AddRoom(number, type, price);
            Console.WriteLine("Додано: " + room.GetInfo());
        }

        private static void ChangeRoomPrice()
        {
            Room room = ReadExistingRoom();
            decimal price = ReadDecimal("Нова ціна за ніч, грн: ");

            room.PricePerNight = price;
            Console.WriteLine("Змінено: " + room.GetInfo());
        }

        private static void RemoveRoom()
        {
            int number = ReadInt("Номер кімнати для видалення: ");

            if (_hotel.RemoveRoom(number))
            {
                Console.WriteLine($"Номер {number} видалено.");
            }
            else
            {
                Console.WriteLine($"Номер {number} не знайдено.");
            }
        }

        private static void GuestsMenu()
        {
            ShowMenu("ГОСТІ",
                "1. Показати всіх гостей",
                "2. Зареєструвати гостя",
                "3. Бронювання гостя (за телефоном)",
                "0. Назад");

            switch (ReadChoice())
            {
                case "1": ShowGuests(); break;
                case "2": RegisterGuest(); break;
                case "3": ShowGuestBookings(); break;
                case "0": break;
                default: Console.WriteLine("Невірний пункт меню!"); break;
            }
        }

        private static void ShowGuests()
        {
            if (_hotel.Guests.Count == 0)
            {
                Console.WriteLine("Гостей ще немає.");
                return;
            }

            for (int i = 0; i < _hotel.Guests.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {_hotel.Guests[i].GetInfo()}");
            }
        }

        private static void RegisterGuest()
        {
            string firstName = ReadText("Ім'я: ");
            string lastName = ReadText("Прізвище: ");
            string phone = ReadText("Телефон: ");

            Guest guest = new Guest(firstName, lastName, phone);
            _hotel.RegisterGuest(guest);

            Console.WriteLine(guest.GetGreeting());
            Console.WriteLine("Зареєстровано: " + guest.GetInfo());
        }

        private static void ShowGuestBookings()
        {
            Guest guest = ReadExistingGuest();

            Console.WriteLine(guest.GetInfo());

            if (guest.Bookings.Count == 0)
            {
                Console.WriteLine("  Бронювань немає.");
                return;
            }

            foreach (Booking booking in guest.Bookings)
            {
                Console.WriteLine("  " + booking.GetInfo());
            }
        }

        private static void EmployeesMenu()
        {
            ShowMenu("ПРАЦІВНИКИ",
                "1. Показати всіх працівників",
                "2. Найняти працівника",
                "3. Підвищити зарплату",
                "4. Звільнити працівника",
                "0. Назад");

            switch (ReadChoice())
            {
                case "1": ShowEmployees(); break;
                case "2": HireEmployee(); break;
                case "3": RaiseSalary(); break;
                case "4": FireEmployee(); break;
                case "0": break;
                default: Console.WriteLine("Невірний пункт меню!"); break;
            }
        }

        private static void ShowEmployees()
        {
            if (_hotel.Employees.Count == 0)
            {
                Console.WriteLine("Працівників ще немає.");
                return;
            }

            for (int i = 0; i < _hotel.Employees.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {_hotel.Employees[i].GetInfo()}");
            }
        }

        private static void HireEmployee()
        {
            string firstName = ReadText("Ім'я: ");
            string lastName = ReadText("Прізвище: ");
            string phone = ReadText("Телефон: ");
            Position position = ReadPosition();
            decimal salary = ReadDecimal("Зарплата, грн: ");

            Employee employee = new Employee(firstName, lastName, phone, position, salary);
            _hotel.HireEmployee(employee);

            Console.WriteLine(employee.GetGreeting());
            Console.WriteLine("Найнято: " + employee.GetInfo());
        }

        private static void RaiseSalary()
        {
            Employee employee = ReadExistingEmployee();
            decimal percent = ReadDecimal("Відсоток підвищення: ");

            employee.RaiseSalary(percent);
            Console.WriteLine("Нова зарплата: " + employee.GetInfo());
        }

        private static void FireEmployee()
        {
            Employee employee = ReadExistingEmployee();

            _hotel.FireEmployee(employee);
            Console.WriteLine($"Працівника {employee.GetFullName()} звільнено.");
        }

        private static void BookingsMenu()
        {
            ShowMenu("БРОНЮВАННЯ",
                "1. Показати всі бронювання",
                "2. Створити бронювання",
                "3. Підтвердити бронювання",
                "4. Заселити гостя",
                "5. Оплатити бронювання",
                "6. Виселити гостя",
                "7. Скасувати бронювання",
                "0. Назад");

            switch (ReadChoice())
            {
                case "1": ShowBookings(); break;
                case "2": CreateBooking(); break;
                case "3": ConfirmBooking(); break;
                case "4": CheckInBooking(); break;
                case "5": PayBooking(); break;
                case "6": CheckOutBooking(); break;
                case "7": CancelBooking(); break;
                case "0": break;
                default: Console.WriteLine("Невірний пункт меню!"); break;
            }
        }

        private static void ShowBookings()
        {
            if (_hotel.Bookings.Count == 0)
            {
                Console.WriteLine("Бронювань ще немає.");
                return;
            }

            for (int i = 0; i < _hotel.Bookings.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {_hotel.Bookings[i].GetInfo()}");
            }
        }

        private static void CreateBooking()
        {
            Guest guest = ReadExistingGuest();
            int number = ReadInt("Номер кімнати: ");
            DateTime checkIn = ReadDate("Дата заїзду (дд.мм.рррр): ");
            DateTime checkOut = ReadDate("Дата виїзду (дд.мм.рррр): ");

            Booking booking = _hotel.CreateBooking(guest, number, checkIn, checkOut);
            Console.WriteLine("Створено: " + booking.GetInfo());
        }

        private static void ConfirmBooking()
        {
            Booking booking = ReadExistingBooking();

            booking.Confirm();
            Console.WriteLine("Бронювання підтверджено.");
            Console.WriteLine("  " + booking.GetInfo());
        }

        private static void CheckInBooking()
        {
            Booking booking = ReadExistingBooking();

            booking.CheckInGuest();
            Console.WriteLine("Гостя заселено.");
            Console.WriteLine("  " + booking.GetInfo());
        }

        private static void CheckOutBooking()
        {
            Booking booking = ReadExistingBooking();

            booking.CheckOutGuest();
            Console.WriteLine("Гостя виселено.");
            Console.WriteLine("  " + booking.GetInfo());
        }

        private static void PayBooking()
        {
            Booking booking = ReadExistingBooking();
            Console.WriteLine($"До сплати: {booking.CalculateTotal():0.##} грн");
            PaymentMethod method = ReadPaymentMethod();

            Payment payment = booking.Pay(method);
            Console.WriteLine(payment.GetInfo());
        }

        private static void CancelBooking()
        {
            Booking booking = ReadExistingBooking();

            _hotel.CancelBooking(booking);
            Console.WriteLine("Бронювання скасовано.");
            Console.WriteLine("  " + booking.GetInfo());
        }

        private static void SearchFreeRooms()
        {
            DateTime from = ReadDate("Дата заїзду (дд.мм.рррр): ");
            DateTime to = ReadDate("Дата виїзду (дд.мм.рррр): ");
            Console.WriteLine("Тип номера: 0 - будь-який, 1 - Single, 2 - Double, 3 - Suite, 4 - Family");
            int typeNumber = ReadInt("Ваш вибір: ");

            List<Room> rooms = typeNumber == 0
                ? _hotel.FindAvailableRooms(from, to)
                : _hotel.FindAvailableRooms(ToRoomType(typeNumber), from, to);

            if (rooms.Count == 0)
            {
                Console.WriteLine("Вільних номерів на цей період немає.");
                return;
            }

            rooms.Sort();
            Console.WriteLine($"Вільних номерів: {rooms.Count}");

            foreach (Room room in rooms)
            {
                Console.WriteLine("  " + room.GetInfo());
            }
        }

        private static void ShowReports()
        {
            Console.WriteLine($"Номерів: {_hotel.Rooms.Count}, гостей: {_hotel.Guests.Count}, працівників: {_hotel.Employees.Count}, бронювань: {_hotel.Bookings.Count}");
            Console.WriteLine($"Отримано оплат: {_hotel.CalculateIncome():0.##} грн");
            Console.WriteLine($"Очікуваний дохід за активними бронюваннями: {_hotel.CalculateExpectedIncome():0.##} грн");
            Console.WriteLine($"Витрати на зарплати за місяць: {_hotel.GetSalaryExpenses():0.##} грн");

            DateTime date = ReadDate("Дата для розрахунку зайнятості (дд.мм.рррр): ");
            Console.WriteLine($"Зайнятість номерів на {date.ToString(DateFormat, CultureInfo.InvariantCulture)}: {_hotel.GetOccupancyRate(date):0.##}%");
        }

        private static void DemonstratePolymorphism()
        {
            Console.WriteLine("1) Абстрактний клас Person: змінна базового типу, виклик virtual/abstract членів");
            List<Person> people = _hotel.GetAllPeople();

            if (people.Count == 0)
            {
                Console.WriteLine("  Немає гостей і працівників.");
            }

            foreach (Person person in people)
            {
                Console.WriteLine($"  [{person.GetType().Name}] {person.Role}");
                Console.WriteLine($"    GetGreeting(): {person.GetGreeting()}");
                Console.WriteLine($"    GetInfo():     {person.GetInfo()}");
            }

            Console.WriteLine();
            Console.WriteLine("2) Змінна типу інтерфейсу IPrintable: один метод GetInfo() - різні реалізації");

            foreach (IPrintable item in _hotel.GetPrintableItems())
            {
                Console.WriteLine($"  [{item.GetType().Name}] {item.GetInfo()}");
            }

            Console.WriteLine();
            Console.WriteLine("3) Змінна типу інтерфейсу IPayable: сума до оплати");

            foreach (Booking booking in _hotel.Bookings)
            {
                IPayable payable = booking;
                Console.WriteLine($"  [{payable.GetType().Name}] номер {booking.Room.Number}: {payable.CalculateTotal():0.##} грн");
            }

            Console.WriteLine();
            Console.WriteLine("4) IEnumerable<Room>: foreach безпосередньо по об'єкту Hotel");

            foreach (Room room in _hotel)
            {
                Console.WriteLine("  " + room.GetInfo());
            }
        }

        private static void FillDemoData()
        {
            if (_hotel.Rooms.Count > 0 || _hotel.Guests.Count > 0 || _hotel.Employees.Count > 0)
            {
                Console.WriteLine("Демо-дані можна додати тільки в порожній готель.");
                return;
            }

            _hotel.AddRoom(201, RoomType.Suite, 3000m);
            _hotel.AddRoom(101, RoomType.Single, 800m);
            _hotel.AddRoom(102, RoomType.Double, 1200m);
            _hotel.AddRoom(301, RoomType.Family, 2000m);

            _hotel.HireEmployee(new Employee("Олена", "Коваль", "0671234567", Position.Receptionist, 20000m));
            _hotel.HireEmployee(new Employee("Петро", "Сидоренко", "0931234567", Position.Manager, 35000m));

            Guest ivan = new Guest("Іван", "Петренко", "0501234567");
            Guest maria = new Guest("Марія", "Бондар", "0661234567");
            _hotel.RegisterGuest(ivan);
            _hotel.RegisterGuest(maria);

            Booking first = _hotel.CreateBooking(ivan, 102, new DateTime(2026, 11, 20), new DateTime(2026, 11, 23));
            first.Confirm();
            _hotel.CreateBooking(maria, 201, new DateTime(2026, 11, 21), new DateTime(2026, 11, 22));

            Console.WriteLine("Додано 4 номери, 2 працівників, 2 гостей і 2 бронювання.");
        }

        private static void PrintError(Exception ex)
        {
            Console.WriteLine("Помилка: " + GetUserMessage(ex));
        }

        private static string GetUserMessage(Exception ex)
        {
            string message = ex.Message;
            int index = message.IndexOf(" (Parameter", StringComparison.Ordinal);
            return index > 0 ? message.Substring(0, index) : message;
        }

        private static void ShowMenu(string title, params string[] items)
        {
            Console.WriteLine();
            Console.WriteLine("================================");
            Console.WriteLine("  " + title);
            Console.WriteLine("================================");

            foreach (string item in items)
            {
                Console.WriteLine(item);
            }
        }

        private static string ReadChoice()
        {
            Console.Write("Ваш вибір: ");
            return (Console.ReadLine() ?? string.Empty).Trim();
        }

        private static string ReadText(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        private static int ReadInt(string prompt)
        {
            string text = ReadText(prompt).Trim();

            if (!int.TryParse(text, out int value))
            {
                throw new FormatException("Введіть ціле число!");
            }

            return value;
        }

        private static decimal ReadDecimal(string prompt)
        {
            string text = ReadText(prompt).Trim().Replace(',', '.');

            if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
            {
                throw new FormatException("Введіть число!");
            }

            return value;
        }

        private static DateTime ReadDate(string prompt)
        {
            string text = ReadText(prompt).Trim();

            if (!DateTime.TryParseExact(text, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value))
            {
                throw new FormatException("Введіть дату у форматі дд.мм.рррр!");
            }

            return value;
        }

        private static RoomType ReadRoomType()
        {
            Console.WriteLine("Тип номера: 1 - Single, 2 - Double, 3 - Suite, 4 - Family");
            return ToRoomType(ReadInt("Ваш вибір: "));
        }

        private static RoomType ToRoomType(int number)
        {
            RoomType type = (RoomType)(number - 1);

            if (!Enum.IsDefined(typeof(RoomType), type))
            {
                throw new ArgumentException("Невірний тип номера!");
            }

            return type;
        }

        private static Position ReadPosition()
        {
            Console.WriteLine("Посада: 1 - Receptionist, 2 - Manager, 3 - Housekeeper, 4 - Cook");
            Position position = (Position)(ReadInt("Ваш вибір: ") - 1);

            if (!Enum.IsDefined(typeof(Position), position))
            {
                throw new ArgumentException("Невірна посада!");
            }

            return position;
        }

        private static PaymentMethod ReadPaymentMethod()
        {
            Console.WriteLine("Спосіб оплати: 1 - Cash, 2 - Card");
            PaymentMethod method = (PaymentMethod)(ReadInt("Ваш вибір: ") - 1);

            if (!Enum.IsDefined(typeof(PaymentMethod), method))
            {
                throw new ArgumentException("Невірний спосіб оплати!");
            }

            return method;
        }

        private static Room ReadExistingRoom()
        {
            int number = ReadInt("Номер кімнати: ");
            return _hotel.FindRoom(number) ?? throw new InvalidOperationException($"Номер {number} не знайдено!");
        }

        private static Guest ReadExistingGuest()
        {
            string phone = ReadText("Телефон гостя: ");
            return _hotel.FindGuestByPhone(phone) ?? throw new InvalidOperationException("Гостя з таким телефоном не знайдено!");
        }

        private static Employee ReadExistingEmployee()
        {
            ShowEmployees();
            int index = ReadInt("Номер працівника у списку: ");

            if (index < 1 || index > _hotel.Employees.Count)
            {
                throw new InvalidOperationException("Працівника з таким номером немає!");
            }

            return _hotel.Employees[index - 1];
        }

        private static Booking ReadExistingBooking()
        {
            ShowBookings();
            int index = ReadInt("Номер бронювання у списку: ");

            if (index < 1 || index > _hotel.Bookings.Count)
            {
                throw new InvalidOperationException("Бронювання з таким номером немає!");
            }

            return _hotel.Bookings[index - 1];
        }
    }
}
