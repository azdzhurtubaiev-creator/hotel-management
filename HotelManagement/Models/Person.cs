using System;

namespace HotelManagement
{
    public abstract class Person : IPerson, IPrintable, IComparable<Person>
    {
        protected Person(string firstName, string lastName, string phone)
        {
            FirstName = CheckName(firstName, nameof(firstName));
            LastName = CheckName(lastName, nameof(lastName));
            Phone = CheckPhone(phone);
        }

        public string FirstName { get; }

        public string LastName { get; }

        public string Phone { get; }

        public abstract string Role { get; }

        public abstract string GetGreeting();

        public virtual string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public virtual string GetInfo()
        {
            return $"{Role}: {GetFullName()}, тел. {Phone}";
        }

        public int CompareTo(Person? other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = string.Compare(LastName, other.LastName, StringComparison.CurrentCultureIgnoreCase);

            if (result != 0)
            {
                return result;
            }

            return string.Compare(FirstName, other.FirstName, StringComparison.CurrentCultureIgnoreCase);
        }

        public override string ToString()
        {
            return GetInfo();
        }

        private static string CheckName(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Ім'я та прізвище не можуть бути порожніми!", paramName);
            }

            return value.Trim();
        }

        private static string CheckPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Телефон не може бути порожнім!", nameof(phone));
            }

            string value = phone.Trim();
            string digits = value.StartsWith("+") ? value.Substring(1) : value;

            if (digits.Length < 10 || digits.Length > 12)
            {
                throw new ArgumentException("Телефон має містити від 10 до 12 цифр!", nameof(phone));
            }

            foreach (char c in digits)
            {
                if (!char.IsDigit(c))
                {
                    throw new ArgumentException("Телефон може містити тільки цифри і знак + на початку!", nameof(phone));
                }
            }

            return value;
        }
    }
}
