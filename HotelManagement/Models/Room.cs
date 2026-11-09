using System;

namespace HotelManagement
{
    public class Room : IPrintable
    {
        public Room(int number, RoomType type, decimal pricePerNight)
        {
            throw new NotImplementedException();
        }

        public int Number
        {
            get => throw new NotImplementedException();
        }

        public RoomType Type
        {
            get => throw new NotImplementedException();
        }

        public decimal PricePerNight
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public int Capacity
        {
            get => throw new NotImplementedException();
        }

        public string GetInfo()
        {
            throw new NotImplementedException();
        }
    }
}
