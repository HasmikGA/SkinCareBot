using LinqToDB;
using LinqToDB.Data;
using SkinCareBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.DataAccess
{
    internal class DbContext : DataConnection
    {
        public ITable<User> Users => this.GetTable<User>();
        public ITable<Product> Products => this.GetTable<Product>();
        public ITable<Recommandation> Reccomendations  => this.GetTable<Recommandation>();
        public DbContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString) { }
    }
}
