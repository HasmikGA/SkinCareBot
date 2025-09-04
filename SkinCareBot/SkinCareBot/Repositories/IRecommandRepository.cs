using SkinCareBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Repositories
{
    internal interface IRecommandRepository
    {
        Task Add(Recommandation recommendation, CancellationToken ct);
        Task Update(Recommandation recommendation, CancellationToken ct);
        Task Delete(Guid id, CancellationToken ct);
        Task<Recommandation> Get(Guid id, CancellationToken ct);
        Task <IReadOnlyList<Recommandation>> GetAll(SkinType skinType, CancellationToken ct);
    }
}
