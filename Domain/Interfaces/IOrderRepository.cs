using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using System;

namespace Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Task<Order> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync(); 
        Task UpdateAsync(Order order);
        Task<bool> SoftDeleteAsync(int id);
        Task<IEnumerable<Order>> SearchOrders(DateTime? startDate, DateTime? endDate, decimal? minPrice, decimal? maxPrice, int? productId);
    }
}