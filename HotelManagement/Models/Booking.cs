using System;

namespace HotelManagement
{
    public class Booking : IPayable, IPrintable
    {
        private Guest _guest;

        private Room _room;

        private Payment? _payment;

        public Booking(Guest guest, Room room, DateTime checkIn, DateTime checkOut)
        {
            throw new NotImplementedException();
        }

        public Guest Guest
        {
            get => throw new NotImplementedException();
        }

        public Room Room
        {
            get => throw new NotImplementedException();
        }

        public DateTime CheckIn
        {
            get => throw new NotImplementedException();
        }

        public DateTime CheckOut
        {
            get => throw new NotImplementedException();
        }

        public BookingStatus Status
        {
            get => throw new NotImplementedException();
        }

        public Payment? Payment
        {
            get => throw new NotImplementedException();
        }

        public bool IsPaid
        {
            get => throw new NotImplementedException();
        }

        public bool IsActive
        {
            get => throw new NotImplementedException();
        }

        public int GetNights()
        {
            throw new NotImplementedException();
        }

        public decimal CalculateTotal()
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public void Confirm()
        {
            throw new NotImplementedException();
        }

        public void CheckInGuest()
        {
            throw new NotImplementedException();
        }

        public void CheckOutGuest()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public Payment Pay(PaymentMethod method)
        {
            throw new NotImplementedException();
        }

        public string GetInfo()
        {
            throw new NotImplementedException();
        }
    }
}
