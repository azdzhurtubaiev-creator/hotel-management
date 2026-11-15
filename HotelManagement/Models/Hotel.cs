using System;
using System.Collections;
using System.Collections.Generic;

namespace HotelManagement
{
    public class Hotel : IEnumerable<Room>
    {
        private readonly List<Room> _rooms = new List<Room>();
        private readonly List<Employee> _employees = new List<Employee>();
        private readonly List<Guest> _guests = new List<Guest>();
        private readonly List<Booking> _bookings = new List<Booking>();
        private int _stars;

        public Hotel(string name, int stars, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Назва готелю не може бути порожньою!", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Адреса готелю не може бути порожньою!", nameof(address));
            }

            Name = name.Trim();
            Address = address.Trim();
            Stars = stars;
        }

        public string Name { get; }

        public string Address { get; }

        public int Stars
        {
            get => _stars;
            set
            {
                if (value < 1 || value > 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(Stars), "Кількість зірок має бути від 1 до 5!");
                }

                _stars = value;
            }
        }

        public IReadOnlyList<Room> Rooms
        {
            get => _rooms.AsReadOnly();
        }

        public IReadOnlyList<Employee> Employees
        {
            get => _employees.AsReadOnly();
        }

        public IReadOnlyList<Guest> Guests
        {
            get => _guests.AsReadOnly();
        }

        public IReadOnlyList<Booking> Bookings
        {
            get => _bookings.AsReadOnly();
        }

        public Room AddRoom(int number, RoomType type, decimal pricePerNight)
        {
            if (FindRoom(number) != null)
            {
                throw new InvalidOperationException($"Номер {number} вже існує!");
            }

            Room room = new Room(number, type, pricePerNight);
            _rooms.Add(room);
            return room;
        }

        public Room? FindRoom(int number)
        {
            return _rooms.Find(r => r.Number == number);
        }

        public bool RemoveRoom(int number)
        {
            Room? room = FindRoom(number);

            if (room == null)
            {
                return false;
            }

            if (_bookings.Exists(b => b.Room == room && b.IsActive))
            {
                throw new InvalidOperationException("Не можна видалити номер з активним бронюванням!");
            }

            return _rooms.Remove(room);
        }

        public List<Room> GetSortedRooms()
        {
            List<Room> rooms = new List<Room>(_rooms);
            rooms.Sort();
            return rooms;
        }

        public void HireEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Працівник не може бути відсутнім!");
            }

            if (_employees.Contains(employee))
            {
                throw new InvalidOperationException("Цей працівник уже працює в готелі!");
            }

            _employees.Add(employee);
        }

        public bool FireEmployee(Employee employee)
        {
            return _employees.Remove(employee);
        }

        public void RegisterGuest(Guest guest)
        {
            if (guest == null)
            {
                throw new ArgumentNullException(nameof(guest), "Гість не може бути відсутнім!");
            }

            if (_guests.Contains(guest))
            {
                throw new InvalidOperationException("Цей гість уже зареєстрований!");
            }

            _guests.Add(guest);
        }

        public Guest? FindGuestByPhone(string phone)
        {
            string value = (phone ?? string.Empty).Trim();
            return _guests.Find(g => g.Phone == value);
        }

        public List<Person> GetAllPeople()
        {
            List<Person> people = new List<Person>();
            people.AddRange(_guests);
            people.AddRange(_employees);
            people.Sort();
            return people;
        }

        public List<IPrintable> GetPrintableItems()
        {
            List<IPrintable> items = new List<IPrintable>();
            items.AddRange(_rooms);
            items.AddRange(_guests);
            items.AddRange(_employees);
            items.AddRange(_bookings);
            return items;
        }

        public bool IsRoomAvailable(int roomNumber, DateTime from, DateTime to)
        {
            Room room = FindRoom(roomNumber) ?? throw new InvalidOperationException($"Номер {roomNumber} не знайдено!");

            if (to.Date <= from.Date)
            {
                throw new ArgumentException("Кінець періоду має бути пізніше початку!");
            }

            return !_bookings.Exists(b => b.Room == room && b.IsActive && b.Overlaps(from, to));
        }

        public List<Room> FindAvailableRooms(DateTime from, DateTime to)
        {
            if (to.Date <= from.Date)
            {
                throw new ArgumentException("Кінець періоду має бути пізніше початку!");
            }

            return _rooms.FindAll(r => IsRoomAvailable(r.Number, from, to));
        }

        public List<Room> FindAvailableRooms(RoomType type, DateTime from, DateTime to)
        {
            return FindAvailableRooms(from, to).FindAll(r => r.Type == type);
        }

        public Booking CreateBooking(Guest guest, int roomNumber, DateTime checkIn, DateTime checkOut)
        {
            if (guest == null)
            {
                throw new ArgumentNullException(nameof(guest), "Гість не може бути відсутнім!");
            }

            if (!_guests.Contains(guest))
            {
                throw new InvalidOperationException("Гість не зареєстрований у готелі!");
            }

            Room room = FindRoom(roomNumber) ?? throw new InvalidOperationException($"Номер {roomNumber} не знайдено!");
            Booking booking = new Booking(guest, room, checkIn, checkOut);

            if (!IsRoomAvailable(roomNumber, checkIn, checkOut))
            {
                throw new InvalidOperationException($"Номер {roomNumber} уже зайнятий на ці дати!");
            }

            _bookings.Add(booking);
            guest.AddBooking(booking);
            return booking;
        }

        public bool CancelBooking(Booking booking)
        {
            if (booking == null || !_bookings.Contains(booking))
            {
                return false;
            }

            booking.Cancel();
            return true;
        }

        public decimal CalculateIncome()
        {
            decimal income = 0;

            foreach (Booking booking in _bookings)
            {
                if (booking.Payment != null)
                {
                    income += booking.Payment.Amount;
                }
            }

            return income;
        }

        public decimal CalculateExpectedIncome()
        {
            decimal total = 0;
            List<IPayable> payables = new List<IPayable>();
            payables.AddRange(_bookings.FindAll(b => b.IsActive));

            foreach (IPayable payable in payables)
            {
                total += payable.CalculateTotal();
            }

            return total;
        }

        public decimal GetSalaryExpenses()
        {
            decimal total = 0;

            foreach (Employee employee in _employees)
            {
                total += employee.Salary;
            }

            return total;
        }

        public double GetOccupancyRate(DateTime date)
        {
            if (_rooms.Count == 0)
            {
                return 0;
            }

            int busy = 0;

            foreach (Room room in _rooms)
            {
                if (_bookings.Exists(b => b.Room == room && b.IsActive && b.Overlaps(date, date.AddDays(1))))
                {
                    busy++;
                }
            }

            return Math.Round(busy * 100.0 / _rooms.Count, 2);
        }

        public IEnumerator<Room> GetEnumerator()
        {
            return _rooms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
