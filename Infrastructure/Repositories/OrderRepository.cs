using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System.Text;


namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbConnectionFactory _dbFactory;

        public OrderRepository(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            using var db = _dbFactory.CreateConnection();
            string sql = "SELECT * FROM Orders WHERE DeleteDate IS NULL";
            var orders = await db.QueryAsync<Order>(sql);

            foreach (var order in orders)
            {
                string itemsSql = "SELECT * FROM OrderItems WHERE OrderId = @Id";
                order.OrderItems = (await db.QueryAsync<OrderItem>(itemsSql, new { Id = order.Id })).ToList();
            }

            return orders;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            using var db = _dbFactory.CreateConnection();
            
            var order = await GetByIdAsync(id);
            if (order == null)
                return false;
            
            string sql = "UPDATE Orders SET DeleteDate = @DeleteDate WHERE Id = @Id";
            await db.ExecuteAsync(sql, new { Id = id, DeleteDate = DateTime.UtcNow });
            
            foreach (var item in order.OrderItems)
            {
                string updateStockSql = "UPDATE Products SET StockQuantity = StockQuantity + @Quantity WHERE Id = @ProductId";
                await db.ExecuteAsync(updateStockSql, new { item.ProductId, item.Quantity });
            }

            return true;
        }


        public async Task<Order> AddAsync(Order order)
        {
            using var db = _dbFactory.CreateConnection();
            string sql = @"
                INSERT INTO Orders (OrderDate, TotalPrice, DeleteDate) 
                VALUES (@OrderDate, @TotalPrice, @DeleteDate) RETURNING Id";
            order.Id = await db.ExecuteScalarAsync<int>(sql, order);

            foreach (var item in order.OrderItems)
            {
                string itemSql = @"
                    INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price) 
                    VALUES (@OrderId, @ProductId, @Quantity, @Price) RETURNING Id";
                item.Id = await db.ExecuteScalarAsync<int>(itemSql, new { OrderId = order.Id, item.ProductId, item.Quantity, item.Price });
            }

            return order;
        }
        public async Task UpdateAsync(Order order)
        {
            using var db = _dbFactory.CreateConnection();
            string sql = "UPDATE Orders SET DeleteDate = @DeleteDate WHERE Id = @Id";
            await db.ExecuteAsync(sql, order);
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            using var db = _dbFactory.CreateConnection();

            string sql = "SELECT * FROM Orders WHERE Id = @Id AND DeleteDate IS NULL";
            var order = await db.QueryFirstOrDefaultAsync<Order>(sql, new { Id = id });

            if (order != null)
            {
                string itemsSql = @"
                SELECT 
                    oi.Id, 
                    oi.OrderId, 
                    oi.ProductId,
                    oi.Quantity, 
                    oi.Price 
                FROM OrderItems oi
                WHERE oi.OrderId = @Id";

                var orderItems = await db.QueryAsync<OrderItem>(
                    itemsSql, 
                    new { Id = id });
                
                order.OrderItems = orderItems.ToList();
            }

            return order;
        }
        
        public async Task<IEnumerable<Order>> SearchOrders(DateTime? startDate, DateTime? endDate, decimal? minPrice, decimal? maxPrice, int? productId)
        {
            using var db = _dbFactory.CreateConnection();

            var query = new StringBuilder(@"
        SELECT DISTINCT o.*
        FROM Orders o
        LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
        LEFT JOIN Products p ON oi.ProductId = p.Id
        WHERE o.DeleteDate IS NULL AND p.DeleteDate IS NULL
    ");

            var parameters = new DynamicParameters();

            if (startDate.HasValue)
            {
                query.Append(" AND o.OrderDate >= @StartDate");
                parameters.Add("StartDate", startDate.Value);
            }
            if (endDate.HasValue)
            {
                query.Append(" AND o.OrderDate <= @EndDate");
                parameters.Add("EndDate", endDate.Value);
            }
            if (minPrice.HasValue)
            {
                query.Append(" AND o.TotalPrice >= @MinPrice");
                parameters.Add("MinPrice", minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query.Append(" AND o.TotalPrice <= @MaxPrice");
                parameters.Add("MaxPrice", maxPrice.Value);
            }
            if (productId.HasValue)
            {
                query.Append(" AND oi.ProductId = @ProductId");
                parameters.Add("ProductId", productId.Value);
            }

            var orders = await db.QueryAsync<Order>(query.ToString(), parameters);
    
            foreach (var order in orders)
            {
                var orderItems = await db.QueryAsync<OrderItem>(
                    "SELECT * FROM OrderItems WHERE OrderId = @Id",
                    new { Id = order.Id });

                order.OrderItems = orderItems.ToList();
            }

            return orders;
        }

    }
}
