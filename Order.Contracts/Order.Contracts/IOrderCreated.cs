using System;

namespace Order.Contracts
{
    public interface IOrderCreated
    {
         Guid OrderId { get; set; }
    }
}