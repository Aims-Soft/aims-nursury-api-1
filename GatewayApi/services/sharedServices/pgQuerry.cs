
using System.Collections.Generic;
// using System.Data;
// using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Npgsql;

namespace gatewayapi.Services
{
    public class pgQuery
    {

        public static IEnumerable<T> Qry<T>(string sql, string conStr)
        {

            using (NpgsqlConnection con = new NpgsqlConnection(conStr))
            {
                return con.Query<T>(sql).ToList();
            }
        }
    }
}