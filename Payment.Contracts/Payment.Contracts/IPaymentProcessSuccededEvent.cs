using System;

namespace Payment.Contracts
{
    public interface IPaymentProcessSuccededEvent
    {
        Guid? PaymentId { get; set; }
        Guid OrderId { get; set; }
    }
}