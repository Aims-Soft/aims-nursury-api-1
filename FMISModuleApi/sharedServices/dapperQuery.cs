
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Options;
using FMISModuleApi.Configuration;
using FMISModuleApi.Entities;
using Npgsql;

namespace FMISModuleApi.Services
{
    public class dapperQuery
    {
        public static IEnumerable<T> Qry<T>(string sql, IOptions<conStr> conStr)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(conStr.Value.dbCon))
            {
                return con.Query<T>(sql);
            }
        }

        public static IEnumerable<T> QryResult<T>(string sql, IOptions<conStr> conStr)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(conStr.Value.dbCon))
            {
                return con.Query<T>(sql).ToList();
            }
        }
        public static IEnumerable<int> CRUDQry(string query, DynamicParameters parameters, IOptions<conStr> conStr)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(conStr.Value.dbCon))
            {
                var rowAffected = con.Execute(query, parameters, commandType: CommandType.Text);

                yield return rowAffected;
            }
        }
    }
}