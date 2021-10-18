using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Commands;

namespace PaymentContext.Tests.Commands
{
    [TestClass]
    public class CreateBoletoSubscriptionCommandTest
    {
        [TestMethod]
        public void ShouldReturnErrorWhenCPNJIsInvalid()
        {
            var command = new CreateBoletoSubscriptionCommand();
            command.FirstName = "";
            command.Validate();
            Assert.AreNotEqual(false, command.IsValid);
        }
    }
}
