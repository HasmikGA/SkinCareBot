using SkinCareBot.Entities;
using SkinCareBot.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<User>? GetUser(long telegramUserId, CancellationToken ct)
        {
            var user = await this.userRepository.GetUserByTelegramUserId(telegramUserId, ct);
            return user;
        }

        public async Task<User> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                TelegramUserName = telegramUserName,
                RegisteredAt = DateTime.Now,
                IsAdmin = false
            };

            await userRepository.Add(user, ct);

            return user;
        }
    }
}
