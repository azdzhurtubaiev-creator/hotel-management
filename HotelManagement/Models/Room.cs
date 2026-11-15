using System;

namespace HotelManagement
{
    public class Room : IPrintable, IComparable<Room>
    {
        private decimal _pricePerNight;

        public Room(int number, RoomType type, decimal pricePerNight)
        {
            if (number <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Номер кімнати має бути більше 0!");
            }

            if (!Enum.IsDefined(typeof(RoomType), type))
            {
                throw new ArgumentException("Обрано неіснуючий тип номера!", nameof(type));
            }

            Number = number;
            Type = type;
            PricePerNight = pricePerNight;
        }

        public int Number { get; }

        public RoomType Type { get; }

        public decimal PricePerNight
        {
            get => _pricePerNight;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(PricePerNight), "Ціна за ніч має бути більше 0!");
                }

                _pricePerNight = value;
            }
        }

        public int Capacity
        {
            get
            {
                return Type switch
                {
                    RoomType.Single => 1,
                    RoomType.Family => 4,
                    _ => 2
                };
            }
        }

        public string GetInfo()
        {
            return $"Номер {Number} ({Type}), місць: {Capacity}, {PricePerNight:0.##} грн/ніч";
        }

        public int CompareTo(Room? other)
        {
            if (other == null)
            {
                return 1;
            }

            return Number.CompareTo(other.Number);
        }

        public override string ToString()
        {
            return GetInfo();
        }
    }
}
