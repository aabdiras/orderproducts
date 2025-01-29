using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data; 
namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbConnectionFactory _dbFactory;

        public ProductRepository(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        private IDbConnection Connection => _dbFactory.CreateConnection();

        public async Task<Product> GetByIdAsync(int id)
        {
            using var db = Connection;
            string sql = "SELECT * FROM Products WHERE Id = @Id AND DeleteDate IS NULL";
            var product = await db.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
            
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var db = Connection;
            string sql = "SELECT * FROM Products WHERE DeleteDate IS NULL";
            return await db.QueryAsync<Product>(sql);
        }

        public async Task<Product> AddAsync(Product product)
        {
            using var db = Connection;
            string sql = "INSERT INTO Products (Name, Price, StockQuantity) VALUES (@Name, @Price, @StockQuantity) RETURNING Id";
            product.Id =  await db.ExecuteAsync(sql, product);

            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            using var db = Connection;
            string sql = "UPDATE Products SET Name = @Name, Price = @Price, StockQuantity = @StockQuantity WHERE Id = @Id";
            await db.ExecuteAsync(sql, product);
        }

        public async Task SoftDeleteAsync(int id)
        {
            using var db = Connection;
            string sql = "UPDATE Products SET DeleteDate = @DeleteDate WHERE Id = @Id";
            await db.ExecuteAsync(sql, new { Id = id, DeleteDate = DateTime.UtcNow });
        }
    }
}
