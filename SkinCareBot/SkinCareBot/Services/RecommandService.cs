using SkinCareBot.Entities;
using SkinCareBot.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Services
{
    internal class RecommandService : IRecommandService
    {
        private readonly IRecommandRepository recommandRepository;

        public RecommandService(IRecommandRepository recommendRepository)
        {
            this.recommandRepository = recommendRepository;
        }
        public async Task<Recommandation> Add(string text, SkinType skinType, ProductType productType, CancellationToken ct)
        {
            var recommendation = new Recommandation
            {
                Id = Guid.NewGuid(),
                Text = text,
                SkinType = skinType,
                ProductType = productType,
            };
            await this.recommandRepository.Add(recommendation, ct);
            return recommendation;
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            await this.recommandRepository.Delete(id, ct);
        }
        public async Task<Recommandation> Get(Guid id, CancellationToken ct)
        {
            var recommand = await this.recommandRepository.Get(id, ct);
            return recommand;
        }

        public async Task<IReadOnlyList<Recommandation>> GetAllRecommands(SkinType skinType, CancellationToken ct)
        {
            var recommands = await this.recommandRepository.GetAll(skinType, ct);
            return recommands;
        }
        public async Task Update(Recommandation recommendation, CancellationToken ct)
        {
            await this.recommandRepository.Update(recommendation, ct);
        }
    }
}
