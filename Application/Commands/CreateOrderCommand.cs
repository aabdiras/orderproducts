using System;
using System.Collections.Generic;
using Application.DTO;

namespace Application.Commands
{
    public class CreateOrderCommand
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public List<OrderItemDto> OrderItems { get; set; }
    }
}