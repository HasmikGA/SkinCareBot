using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Entities
{
    [Table("User")]
    internal class User
    {
        [PrimaryKey]
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("TelegramUserId"), NotNull]
        public long TelegramUserId { get; set; }

        [Column("TelegramUserName")]
        public string? TelegramUserName { get; set; }

        [Column("RegisteredAt"), NotNull]
        public DateTime RegisteredAt { get; set; }

        [Column ("IsAdmin"), NotNull]
        public bool IsAdmin { get; set; }
    }
}
