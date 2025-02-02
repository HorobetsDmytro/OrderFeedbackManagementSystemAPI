using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Domain.Interfaces;

namespace OrderFeedbackManagementSystemAPI.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found");
            return product;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name is required");

            if (product.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero");

            if (product.Stock < 0)
                throw new ArgumentException("Product stock cannot be negative");

            return await _productRepository.AddAsync(product);
        }

        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                throw new ArgumentException($"Product with ID {id} not found");

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name is required");

            if (product.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero");

            if (product.Stock < 0)
                throw new ArgumentException("Product stock cannot be negative");

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.ImagePath = product.ImagePath;

            await _productRepository.UpdateAsync(existingProduct);
            return existingProduct;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found");

            await _productRepository.DeleteAsync(product);
        }

        public async Task<bool> IsProductInStockAsync(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found");

            return product.Stock >= quantity;
        }

        public async Task<Product> UpdateProductStockAsync(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found");

            if (quantity < 0 && Math.Abs(quantity) > product.Stock)
                throw new InvalidOperationException("Cannot remove more items than available in stock");

            product.Stock += quantity;
            await _productRepository.UpdateAsync(product);
            return product;
        }
    }
}
