using Flunt.Notifications;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;
using System;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandle : Notifiable<Notification>, IHandler<CreateBoletoSubscriptionCommand>, IHandler<CreatePayPalSubscriptionCommand>, IHandler<CreateCreditCardSubscriptionCommand>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IEmailService _emailService;
        public SubscriptionHandle(IStudentRepository studentRepository, IEmailService emailService)
        {
            _studentRepository = studentRepository;
            _emailService = emailService;
        }
        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            command.Validate();

            if (!command.IsValid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possivel realizar a sua assinatura");
            }
            if (_studentRepository.DocumentExists(command.Document))
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possivel realizar a sua assinatura, pois o documento já existe");
            }
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, command.DocumentType);
            var email = new Email(command.Email);

            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(
                command.BarCode,
                command.BoletoNumber,
                command.PaidDate,
                command.ExpireDate,
                command.Total,
                command.TotalPaid,
                command.Payer,
                new Document(command.PayerDocument, command.DocumentType),
                address,
                email);

            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            AddNotifications(name, document, email, address, student, subscription, payment);

            if (!IsValid)
                return new CommandResult(false, "Não foi possível realizar a sua assinatura");

           _studentRepository.CreateSubscription(student);

            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem Vindo ao tiedt.io", "Sua Assinatura foi criada com sucesso!");

            return new CommandResult(true, "Assinatura realizada com sucesso");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            if (_studentRepository.DocumentExists(command.Document))
            {
                return new CommandResult(false, "Não foi possivel realizar a sua assinatura, pois o documento já existe");
            }
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, command.DocumentType);
            var email = new Email(command.Email);

            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(
                command.TransactionCode,
                command.PaidDate,
                command.ExpireDate,
                command.Total,
                command.TotalPaid,
                command.Payer,
                new Document(command.PayerDocument, command.DocumentType),
                address,
                email);

            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            AddNotifications(name, document, email, address, student, subscription, payment);

            if (!IsValid)
                return new CommandResult(false, "Não foi possível realizar a sua assinatura");

            _studentRepository.CreateSubscription(student);

            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem Vindo ao tiedt.io", "Sua Assinatura foi criada com sucesso!");

            return new CommandResult(true, "Assinatura realizada com sucesso");
        }

        public ICommandResult Handle(CreateCreditCardSubscriptionCommand command)
        {
            if (_studentRepository.DocumentExists(command.Document))
            {
                return new CommandResult(false, "Não foi possivel realizar a sua assinatura, pois o documento já existe");
            }
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, command.DocumentType);
            var email = new Email(command.Email);

            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new CreditCardPayment(
                command.CardHolderName,
                command.CardNumber,
                command.LastTransactionNumber,
                command.PaidDate,
                command.ExpireDate,
                command.Total,
                command.TotalPaid,
                command.Payer,
                new Document(command.PayerDocument, command.DocumentType),
                address,
                email);

            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            AddNotifications(name, document, email, address, student, subscription, payment);

            if (!IsValid)
                return new CommandResult(false, "Não foi possível realizar a sua assinatura");

            _studentRepository.CreateSubscription(student);

            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem Vindo ao tiedt.io", "Sua Assinatura foi criada com sucesso!");

            return new CommandResult(true, "Assinatura realizada com sucesso");
        }
    }
}
