using LinqToDB;
using LinqToDB.Concurrency;
using SkinCareBot.DataAccess;
using SkinCareBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Repositories
{
    internal class ProductRepository : IProductRepository
    {
        private readonly IDataContextFactory<DbContext> factory;
        public ProductRepository(IDataContextFactory<DbContext> factory)
        {
            this.factory = factory;
        }

        public async Task Add(Product product, CancellationToken ct)
        {
            using var dbContext = this.factory.CreateDataContext();
            await dbContext.InsertAsync(product, token: ct);
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            using var dbContext = this.factory.CreateDataContext();
            await dbContext.Products.DeleteAsync(i => i.Id == id, token: ct);
        }

        public async Task<Product> Get(Guid id, CancellationToken ct)
        {
            using var dbContext = this.factory.CreateDataContext();
            var product = await dbContext.Products.FirstOrDefaultAsync(i => i.Id == id, ct);
            return product;
        }

        public async Task<IReadOnlyList<Product>> GetAllProducts(ProductType type, SkinType skinType, CancellationToken ct)
        {
            using var dbContext = this.factory.CreateDataContext();
            var products = await dbContext.Products.Where(i=>i.Type == type && i.SkinType == skinType).ToListAsync(ct);
            return products;
        }

        public async Task Update(Product product, CancellationToken ct)
        {
            using var dbContext = this.factory.CreateDataContext();
            await dbContext.UpdateAsync(product, token: ct);
        }
    }
}
