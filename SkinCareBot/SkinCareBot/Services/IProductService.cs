using SkinCareBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Services
{
    internal interface IProductService
    {
        Task<Product> Add(string name, ProductType type, SkinType skinType, CancellationToken ct);

        Task Delete (Guid  id, CancellationToken ct);

        Task Update(Product product, CancellationToken ct);
        Task<Product> Get(Guid id, CancellationToken ct);
        Task<IReadOnlyList<Product>> GetAllProducts(ProductType type, SkinType skinType, CancellationToken ct);

    }
}
