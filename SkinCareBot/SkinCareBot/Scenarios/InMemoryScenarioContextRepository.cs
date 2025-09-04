using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Scenarios
{
    internal class InMemoryScenarioContextRepository: IScenarioContextRepository
    {
        private readonly ConcurrentDictionary<long, ScenarioContext> scenarios = new ConcurrentDictionary<long, ScenarioContext>();
        public Task<bool> HasContext(long userId, CancellationToken ct)
        {
            return Task.FromResult(this.scenarios.ContainsKey(userId));
        }

        public Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
        {
            ScenarioContext? context = null;
            if (scenarios.ContainsKey(userId))
            {
                context = this.scenarios[userId];
            }

            return Task.FromResult(context);
        }

        public Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
        {
            this.scenarios[userId] = context;
            return Task.CompletedTask;
        }

        public Task ResetContext(long userId, CancellationToken ct)
        {
            this.scenarios.Remove(userId, out ScenarioContext value);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<ScenarioContext>> GetContexts(CancellationToken ct)
        {
            IReadOnlyList<ScenarioContext> list = this.scenarios.Values.ToList();
            return list;
        }
    }
}
