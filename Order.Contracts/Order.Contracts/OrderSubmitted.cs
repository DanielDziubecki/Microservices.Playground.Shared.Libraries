using System;

namespace Order.Contracts
{
    public class OrderSubmitted : IOrderSubmitted
    {
        public Guid OrderId { get; set; }
    }
}