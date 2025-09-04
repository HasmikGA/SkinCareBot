using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Entities
{
    [Table("Recommandation")]
    internal class Recommandation
    {
        [PrimaryKey]
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("Text"), NotNull]
        public string? Text { get; set; }
        [Column("SkinType")]
        public SkinType SkinType { get; set; }

        [Column("ProductType")]
        public ProductType ProductType { get; set; }

    }
}
