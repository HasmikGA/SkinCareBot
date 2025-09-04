using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.DataAccess
{
    internal class DataContextFactory : IDataContextFactory<DbContext>
    {
        private readonly string connectionString;
        public DataContextFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public DbContext CreateDataContext()
        {
            return new DbContext(connectionString);
        }
    }
}
