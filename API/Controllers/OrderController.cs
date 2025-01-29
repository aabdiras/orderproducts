using System;
using System.Linq;
using System.Threading.Tasks;
using Application.DTO;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
             _orderService = orderService;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createDto)
        {
            if (createDto.Items == null || !createDto.Items.Any())
                return BadRequest(new { status = "error", message = "Заказ должен содержать хотя бы один товар" });

            try
            {
                var order = await _orderService.CreateOrder(createDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, new { status = "success", data = order });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
                return NotFound(new { status = "error", message = "Заказ не найден" });

            return Ok(new { status = "success", data = order });
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteOrder(int id)
        {
            var success = await _orderService.SoftDeleteOrder(id);
            if (!success)
                return NotFound(new { status = "error", message = "Заказ не найден" });

            return Ok(new { status = "success", message = "Заказ успешно удалён" });
        }
        
        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? productId)
        {
            var orders = await _orderService.SearchOrders(startDate, endDate, minPrice, maxPrice, productId);

            if (!orders.Any())
                return NotFound(new { status = "error", message = "Заказы не найдены" });

            return Ok(new { status = "success", data = orders });
        }
    }
}
