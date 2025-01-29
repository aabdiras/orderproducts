using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
