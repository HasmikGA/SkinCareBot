using LinqToDB;
using SkinCareBot.DataAccess;
using SkinCareBot.Entities;

namespace SkinCareBot.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly IDataContextFactory<DbContext> factory;
        public UserRepository(IDataContextFactory<DbContext> factory)
        {
            this.factory = factory;
        }
        public async Task Add(User user, CancellationToken ct)
        {
            using (var dbContext = this.factory.CreateDataContext())
            {
                await dbContext.InsertAsync(user, token: ct);
            }
        }

        public async Task<User?> GetUser(Guid userId, CancellationToken ct)
        {
            using (var dbContext = this.factory.CreateDataContext())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(i => i.Id == userId, ct);
                return user;
            }
        }

        public async Task<User?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            using (var dbContext = factory.CreateDataContext())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(i => i.TelegramUserId == telegramUserId, ct);
                return user;
            }
        }

    }
}
