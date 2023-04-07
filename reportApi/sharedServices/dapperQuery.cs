
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Options;
using reportApi.Configuration;
// using reportApi.Entities;
using Npgsql;
using System.IO;

namespace reportApi.Services
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
        public static IEnumerable<T> StrConQry<T>(string sql, string conStr)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(conStr))
            {
                return con.Query<T>(sql).ToList();
            }
        }
        public static string saveImageFile(string regPath, string name, string binData, string ext)
        {
            String path = regPath; //Path
            //Check if directory exist
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            string imageName = name + "." + ext;

            //set the image path
            string imgPath = Path.Combine(path, imageName);

            byte[] imageBytes = Convert.FromBase64String(binData);

            System.IO.File.WriteAllBytes(imgPath, imageBytes);

            return "Ok";
        }
        // public static IEnumerable<int> CRUDQry(string query, DynamicParameters parameters, IOptions<conStr> conStr)
        // {
        //     using (NpgsqlConnection con = new NpgsqlConnection(conStr.Value.dbCon))
        //     {
        //         var rowAffected = con.Execute(query, parameters, commandType: CommandType.Text);

        //         yield return rowAffected;
        //     }
        // }
    }
}