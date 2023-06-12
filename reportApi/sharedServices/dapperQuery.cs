
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Options;
using reportApi.Configuration;
using reportApi.Entities;
using Npgsql;
using System.IO;

namespace reportApi.Services
{
    public class dapperQuery
    {
       private readonly IOptions<conStr> _dbCon;

       public dapperQuery(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

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
        public IEnumerable<T> StrConQry<T>(string sql, int userID,int moduleId)
        
        {

            string subCOnStr = "";
            if (userID != 0 && moduleId !=0)
            {
                subCOnStr = FindMe(userID,moduleId);
            }
            using (NpgsqlConnection con = new NpgsqlConnection(subCOnStr))
            {
                return con.Query<T>(sql).ToList();
            }
        }

        public IEnumerable<T> FindMeQuery<T>(string sql)
        
        {
            
            using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
            {
                return con.Query<T>(sql).ToList();
            }
        }
         
        public string FindMe(int userID,int moduleId)

        {
            try
            {
                var cmd = "Select * from view_getcompany where \"userID\" = "+userID+" and \"applicationModuleID\" = "+moduleId+""; // corrected query string

                
                // var user = (List<dynamicResponse>)FindMeQuery<dynamicResponse>(cmd, _dbCon); // assuming _dapper is properly instantiated
                List<dynamicResponse> user = new List<dynamicResponse>(FindMeQuery<dynamicResponse>(cmd));

                
                // var user = FindMeQuery<dynamicResponse>(cmd,_dbCon);

                var abc = "Host="+user[0].instanceName+";Database="+user[0].dbName+";Port=5432;Username="+user[0].userName+";Password="+user[0].credentials+"";

                if (user.Count > 0)
                {
                    return abc; 
                }
                else
                {
                    return ""; // or return an appropriate value when no results are found
                }
            }
            catch (Exception e)
            {
                // Handle exception
                return null; // or return an appropriate value when an exception occurs
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
    }
}