using SkinCareBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Services
{
    internal interface IUserService
    {
        Task<User> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct);
        Task<User>? GetUser(long telegramUserId, CancellationToken ct);
    }
}
