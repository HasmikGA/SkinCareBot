using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.DataAccess
{
    internal interface IDataContextFactory<TDataContext> where TDataContext : DataConnection
    {
        TDataContext CreateDataContext();
    }
}
