using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{
    public class CreateOrderDto
    {
        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Количество товара должно быть больше 0.")]
        public int Quantity { get; set; }
    }
}