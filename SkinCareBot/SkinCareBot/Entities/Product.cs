using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Entities
{
    [Table("Product")]
    internal class Product
    {
        [PrimaryKey]
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("Name"), NotNull]
        public required string Name { get; set; }

        [Column("Type"), NotNull]
        public ProductType Type { get; set; }

        [Column("SkinType"), NotNull]
        public SkinType SkinType { get; set; }
    }
}
