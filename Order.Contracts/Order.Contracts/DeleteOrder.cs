using System;

namespace Order.Contracts
{
    public class DeleteOrder : IDeleteOrder
    {
        public Guid OrderId { get; set; }
    }
}