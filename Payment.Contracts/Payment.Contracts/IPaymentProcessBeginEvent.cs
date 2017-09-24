using System;

namespace Payment.Contracts
{
    public interface IPaymentProcessBeginEvent
    {
        Guid? PaymentId { get; set; }
        Guid OrderId { get; set; }
    }
}