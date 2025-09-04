using SkinCareBot.Entities;
using SkinCareBot.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Services
{
    internal class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public async Task<Product> Add(string name, ProductType type, SkinType skinType, CancellationToken ct)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type,
                SkinType = skinType,
            };
            await this.productRepository.Add(product, ct);

            return product;
        }
        public async Task Delete(Guid id, CancellationToken ct)
        {
            await this.productRepository.Delete(id, ct);
        }

        public async Task<Product> Get(Guid id, CancellationToken ct)
        {
            var product = await this.productRepository.Get(id, ct);
            return product;
        }

        public async Task<IReadOnlyList<Product>> GetAllProducts(ProductType type, SkinType skinType, CancellationToken ct)
        {
            var products = await this.productRepository.GetAllProducts(type, skinType, ct);
            return products;
        }

        public async Task Update(Product product, CancellationToken ct)
        {
            await this.productRepository.Update(product, ct);
        }
    }
}
