using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalDbExample.Entity
{
    public class ConnectionManager
    {
        public static IDbConnection GetConnection()
        {
            return new System.Data.SqlClient.SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true");
        }
    }
}
