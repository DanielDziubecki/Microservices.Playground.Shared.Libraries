using System;

namespace Payment.Contracts
{
    public interface IPaymentProcessFailsEvent
    {
        Guid? PaymentId { get; set; }
        Guid OrderId { get; set; }
    }
}