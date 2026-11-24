using System;
using System.Globalization;

namespace HotelManagement
{
    public class Booking : IPayable, IPrintable
    {
        private readonly Guest _guest;
        private readonly Room _room;
        private readonly decimal _nightPrice;
        private Payment? _payment;

        public event BookingStatusHandler? StatusChanged;

        public Booking(Guest guest, Room room, DateTime checkIn, DateTime checkOut)
        {
            _guest = guest ?? throw new ArgumentNullException(nameof(guest), "Гість не може бути відсутнім!");
            _room = room ?? throw new ArgumentNullException(nameof(room), "Номер не може бути відсутнім!");

            if (checkOut.Date <= checkIn.Date)
            {
                throw new ArgumentException("Дата виїзду має бути пізніше дати заїзду!");
            }

            _nightPrice = room.PricePerNight;
            CheckIn = checkIn.Date;
            CheckOut = checkOut.Date;
            Status = BookingStatus.Created;
        }

        public Guest Guest
        {
            get => _guest;
        }

        public Room Room
        {
            get => _room;
        }

        public decimal NightPrice
        {
            get => _nightPrice;
        }

        public DateTime CheckIn { get; }

        public DateTime CheckOut { get; }

        public BookingStatus Status { get; private set; }

        public Payment? Payment
        {
            get => _payment;
        }

        public bool IsPaid
        {
            get => _payment != null;
        }

        public bool IsActive
        {
            get => Status != BookingStatus.Cancelled && Status != BookingStatus.CheckedOut;
        }

        public int GetNights()
        {
            return (CheckOut - CheckIn).Days;
        }

        public decimal CalculateTotal()
        {
            return GetNights() * _nightPrice;
        }

        public decimal CalculateTotal(DiscountRule rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule), "Правило знижки не може бути відсутнім!");
            }

            decimal percent = rule(this);

            if (percent < 0 || percent >= 100)
            {
                throw new InvalidOperationException("Відсоток знижки має бути від 0 до 99!");
            }

            return CalculateTotal() * (100 - percent) / 100;
        }

        private void ChangeStatus(BookingStatus newStatus)
        {
            BookingStatus oldStatus = Status;
            Status = newStatus;
            StatusChanged?.Invoke(this, oldStatus, newStatus);
        }

        public bool Overlaps(DateTime from, DateTime to)
        {
            if (to.Date <= from.Date)
            {
                throw new ArgumentException("Кінець періоду має бути пізніше початку!");
            }

            return from.Date < CheckOut && to.Date > CheckIn;
        }

        public void Confirm()
        {
            if (Status != BookingStatus.Created)
            {
                throw new InvalidOperationException("Підтвердити можна тільки щойно створене бронювання!");
            }

            ChangeStatus(BookingStatus.Confirmed);
        }

        public void CheckInGuest()
        {
            if (Status != BookingStatus.Confirmed)
            {
                throw new InvalidOperationException("Заселити можна тільки за підтвердженим бронюванням!");
            }

            ChangeStatus(BookingStatus.CheckedIn);
        }

        public void CheckOutGuest()
        {
            if (Status != BookingStatus.CheckedIn)
            {
                throw new InvalidOperationException("Виселити можна тільки заселеного гостя!");
            }

            if (!IsPaid)
            {
                throw new InvalidOperationException("Перед виселенням бронювання має бути оплачене!");
            }

            ChangeStatus(BookingStatus.CheckedOut);
        }

        public void Cancel()
        {
            if (Status != BookingStatus.Created && Status != BookingStatus.Confirmed)
            {
                throw new InvalidOperationException("Скасувати можна тільки бронювання до заселення!");
            }

            ChangeStatus(BookingStatus.Cancelled);
        }

        public Payment Pay(PaymentMethod method)
        {
            return PayAmount(CalculateTotal(), method);
        }

        public Payment Pay(PaymentMethod method, DiscountRule rule)
        {
            return PayAmount(CalculateTotal(rule), method);
        }

        private Payment PayAmount(decimal amount, PaymentMethod method)
        {
            if (Status == BookingStatus.Cancelled)
            {
                throw new InvalidOperationException("Скасоване бронювання не можна оплатити!");
            }

            if (IsPaid)
            {
                throw new InvalidOperationException("Бронювання вже оплачене!");
            }

            _payment = new Payment(amount, method);
            return _payment;
        }

        public string GetInfo()
        {
            string from = CheckIn.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            string to = CheckOut.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            string paid = IsPaid ? "оплачено" : "не оплачено";

            return $"{_guest.GetFullName()}, номер {_room.Number}, {from} - {to}, {GetNights()} ноч., {CalculateTotal():0.##} грн, {Status}, {paid}";
        }

        public override string ToString()
        {
            return GetInfo();
        }
    }
}
