using System;
using System.Collections.Generic;

namespace HotelManagement
{
    public class Hotel
    {
        private List<Room> _rooms;

        private List<Employee> _employees;

        private List<Guest> _guests;

        private List<Booking> _bookings;

        public Hotel(string name, int stars, string address)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get => throw new NotImplementedException();
        }

        public int Stars
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string Address
        {
            get => throw new NotImplementedException();
        }

        public IReadOnlyList<Room> Rooms
        {
            get => throw new NotImplementedException();
        }

        public IReadOnlyList<Employee> Employees
        {
            get => throw new NotImplementedException();
        }

        public IReadOnlyList<Guest> Guests
        {
            get => throw new NotImplementedException();
        }

        public IReadOnlyList<Booking> Bookings
        {
            get => throw new NotImplementedException();
        }

        public Room AddRoom(int number, RoomType type, decimal pricePerNight)
        {
            throw new NotImplementedException();
        }

        public Room? FindRoom(int number)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRoom(int number)
        {
            throw new NotImplementedException();
        }

        public void HireEmployee(Employee employee)
        {
            throw new NotImplementedException();
        }

        public bool FireEmployee(Employee employee)
        {
            throw new NotImplementedException();
        }

        public void RegisterGuest(Guest guest)
        {
            throw new NotImplementedException();
        }

        public Booking CreateBooking(Guest guest, int roomNumber, DateTime checkIn, DateTime checkOut)
        {
            throw new NotImplementedException();
        }

        public bool CancelBooking(Booking booking)
        {
            throw new NotImplementedException();
        }

        public bool IsRoomAvailable(int roomNumber, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public List<Room> FindAvailableRooms(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public List<Room> FindAvailableRooms(RoomType type, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public decimal CalculateIncome()
        {
            throw new NotImplementedException();
        }

        public decimal GetSalaryExpenses()
        {
            throw new NotImplementedException();
        }

        public double GetOccupancyRate(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
