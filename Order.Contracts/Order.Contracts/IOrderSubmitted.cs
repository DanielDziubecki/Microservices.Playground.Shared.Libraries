using System;

namespace Order.Contracts
{
    public interface IOrderSubmitted
    {
         Guid OrderId { get; set; }
    }
}