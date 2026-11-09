using System;
using HotelManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelManagement.Tests
{
    [TestClass]
    public class PaymentTests
    {
        [TestMethod]
        public void Constructor_ValidValues_SetsProperties()
        {
            // Arrange
            DateTime before = DateTime.Now;

            // Act
            Payment payment = new Payment(2400m, PaymentMethod.Card);

            // Assert
            Assert.AreEqual(2400m, payment.Amount);
            Assert.AreEqual(PaymentMethod.Card, payment.Method);
            Assert.IsTrue(payment.PaidAt >= before && payment.PaidAt <= DateTime.Now);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-50)]
        public void Constructor_AmountNotPositive_ThrowsArgumentOutOfRangeException(int amount)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Payment(amount, PaymentMethod.Cash));
        }

        [TestMethod]
        public void Constructor_UndefinedMethod_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => new Payment(100m, (PaymentMethod)99));
        }
    }
}
