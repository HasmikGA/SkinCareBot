using SkinCareBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Services
{
    internal interface IRecommandService
    {
        Task<Recommandation> Add(string text, SkinType skinType, ProductType productType, CancellationToken ct);

        Task Delete(Guid id, CancellationToken ct);
        Task<Recommandation> Get(Guid id, CancellationToken ct);
        Task<IReadOnlyList<Recommandation>> GetAllRecommands(SkinType skinType, CancellationToken ct);
        Task Update(Recommandation recommendation, CancellationToken ct);
    }
}
