using SkinCareBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Repositories
{
    internal interface IUserRepository
    {
        Task<User?> GetUser(Guid userId, CancellationToken ct);
        Task<User?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct);
        Task Add(User user, CancellationToken ct);
    }
}
