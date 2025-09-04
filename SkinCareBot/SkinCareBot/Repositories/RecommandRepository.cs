using LinqToDB;
using SkinCareBot.DataAccess;
using SkinCareBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Repositories
{
    internal class RecommandRepository : IRecommandRepository
    {
        private readonly IDataContextFactory<DbContext> factory;
        public RecommandRepository(IDataContextFactory<DbContext> factory)
        {
            this.factory = factory;
        }
        public async Task Add(Recommandation recommendation, CancellationToken ct)
        {
            using var dbContext = this.factory.CreateDataContext();
            await dbContext.InsertAsync(recommendation, token: ct);
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            using var dbContext = this.factory.CreateDataContext();
            await dbContext.Reccomendations.DeleteAsync(i => i.Id == id, token: ct);
        }
        public async Task<Recommandation> Get(Guid id, CancellationToken ct)
        {
            using var dbContext = this.factory.CreateDataContext();
            var recommand= await dbContext.Reccomendations.FirstOrDefaultAsync(x=>x.Id == id, token: ct);
            return recommand;
        }
        public async Task<IReadOnlyList<Recommandation>> GetAll(SkinType skinType, CancellationToken ct)
        {
            using var dbContext = this.factory.CreateDataContext();
            var recommends = await dbContext.Reccomendations.Where(i=>i.SkinType == skinType).ToListAsync(ct);
            return recommends;
        }

        public async Task Update(Recommandation recommendation, CancellationToken ct)
        {
            using var dbContext = this.factory.CreateDataContext();
            await dbContext.UpdateAsync(recommendation, token: ct);
        }
    }
}
