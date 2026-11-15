using System;

namespace HotelManagement
{
    public class Payment : IPrintable
    {
        public Payment(decimal amount, PaymentMethod method)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Сума оплати має бути більше 0!");
            }

            if (!Enum.IsDefined(typeof(PaymentMethod), method))
            {
                throw new ArgumentException("Обрано неіснуючий спосіб оплати!", nameof(method));
            }

            Amount = amount;
            Method = method;
            PaidAt = DateTime.Now;
        }

        public decimal Amount { get; }

        public PaymentMethod Method { get; }

        public DateTime PaidAt { get; }

        public string GetInfo()
        {
            return $"Оплата {Amount:0.##} грн ({Method}), {PaidAt:dd.MM.yyyy HH:mm}";
        }
    }
}
