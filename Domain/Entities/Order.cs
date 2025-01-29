using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public DateTime? DeleteDate { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        //public Product Product { get; set; } = new Product(); 
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
