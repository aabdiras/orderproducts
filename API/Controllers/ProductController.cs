using System.Threading.Tasks;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Application.DTO;

namespace API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveProducts()
        {
            var products = await _productService.GetActiveProducts();

            if (products == null)
                return NotFound(new { status = "error", message = "Продукты не найдены" });

            return Ok(new { status = "success", data = products });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteProduct(int id)
        {
            var product = await _productService.GetById(id);
            if (product == null)
                return NotFound(new { status = "error", message = "Продукт не найден" });

            await _productService.SoftDeleteProduct(id);
            return Ok(new { status = "success", message = "Продукт успешно удалён" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
        {
            var existingProduct = await _productService.GetById(id);
            if (existingProduct == null)
                return NotFound(new { status = "error", message = "Продукт не найден" });

            existingProduct.Name = updateDto.Name;
            existingProduct.Price = updateDto.Price;
            existingProduct.StockQuantity = updateDto.StockQuantity;

            await _productService.UpdateProduct(existingProduct);
            return Ok(new { status = "success", message = "Продукт успешно обновлён", data = updateDto });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name) || createDto.Price <= 0 || createDto.StockQuantity < 0)
                return BadRequest(new { status = "error", message = "Некорректные данные" });

            var newProduct = new Product
            {
                Name = createDto.Name,
                Price = createDto.Price,
                StockQuantity = createDto.StockQuantity
            };

            var productId = await _productService.CreateProduct(newProduct);
            return CreatedAtAction(nameof(GetActiveProducts), new { id = productId },
                new { status = "success", data = newProduct });
        }
    }
}
