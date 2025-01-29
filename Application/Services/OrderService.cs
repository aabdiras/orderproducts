using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;
using Domain.Interfaces;
using System.Text.Json;

namespace Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Order> CreateOrder(CreateOrderDto createDto)
        {
            if (createDto.Items == null || !createDto.Items.Any())
                throw new Exception("Заказ должен содержать хотя бы один товар.");

            decimal totalPrice = 0;
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                DeleteDate = null,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in createDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                if (product == null)
                    throw new Exception($"Продукт с ID {item.ProductId} не найден.");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Недостаточно товара на складе: {product.Name} (ID: {product.Id}). Запрашиваемое количество: {item.Quantity}, доступно: {product.StockQuantity}.");

                product.StockQuantity -= item.Quantity;
                await _productRepository.UpdateAsync(product);

                decimal itemTotal = product.Price * item.Quantity;
                totalPrice += itemTotal;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                });
            }

            order.TotalPrice = totalPrice;

            return await _orderRepository.AddAsync(order);
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<bool> SoftDeleteOrder(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            order.DeleteDate = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null )
                {
                    product.StockQuantity += item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
               
            }

            return true;
        }
        
        public async Task<IEnumerable<Order>> SearchOrders(DateTime? startDate, DateTime? endDate, decimal? minPrice, decimal? maxPrice, int? productId)
        {
            return await _orderRepository.SearchOrders(startDate, endDate, minPrice, maxPrice, productId);
        }

    }
}
