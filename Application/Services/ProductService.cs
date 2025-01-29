
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetActiveProducts()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task SoftDeleteProduct(int productId)
        {
            await _productRepository.SoftDeleteAsync(productId);
        }
        
        public async Task UpdateProduct(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }
        
        public async Task<Product> GetById(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }
        
        public async Task<Product> CreateProduct(Product product)
        {
            return await _productRepository.AddAsync(product);
        }
    }
}
