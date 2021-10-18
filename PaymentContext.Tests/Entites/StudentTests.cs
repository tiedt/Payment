using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.ValueObjects;
using System;

namespace PaymentContext.Tests.Entites
{
    [TestClass]
    public class StudentTests
    {
        private readonly Student _student;
        private readonly Subscription _subscription;
        private readonly Name _name;
        private readonly Document _document;
        private readonly Address _address;
        private readonly Email _email;

        public StudentTests()
        {
            _address = new Address("Rua 1", "45", "Bairro Legal", "Triste", "SC", "BR", "89255455");
            _name = new Name("Rennã", "Murilo");
            _document = new Document("35111507795", EDocumentType.CPF);
            _email = new Email("ree_murilo@hotmail.com");
            _student = new Student(_name, _document, _email);
            _subscription = new Subscription(null);
        }
        [TestMethod]
        public void ShouldReturnErrorWhenHadActiveSubscription()
        {
            var subscription = new Subscription(null);
            var payment = new PayPalPayment("12345678", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, "WAYNE CORP", _document, _address, _email);
            subscription.AddPayment(payment);
            _student.AddSubscription(subscription);
            _student.AddSubscription(subscription);

            Assert.IsTrue(!_student.IsValid);
        }

        [TestMethod]
        public void ShouldReturnErrorWhenSubscriptionHasNoPayment()
        {
            var subscription = new Subscription(null);
            _student.AddSubscription(subscription);
            Assert.IsTrue(!_student.IsValid);
        }

        [TestMethod]
        public void ShouldReturnSuccessWhenAddSubscription()
        {
            var subscription = new Subscription(null);
            var payment = new PayPalPayment("12345678", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, "WAYNE CORP", _document, _address, _email);
            subscription.AddPayment(payment);
            _student.AddSubscription(subscription);
            Assert.IsTrue(_student.IsValid);
        }
    }
}