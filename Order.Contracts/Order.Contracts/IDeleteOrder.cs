using System;

namespace Order.Contracts
{
    public interface IDeleteOrder
    {
        Guid OrderId { get; set; }
    }
}