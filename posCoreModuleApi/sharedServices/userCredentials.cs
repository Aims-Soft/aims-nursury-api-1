using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Options;
using posCoreModuleApi.Configuration;
using posCoreModuleApi.Entities;
using Npgsql;
using System.IO;
using posCoreModuleApi.Services;

namespace posCoreModuleApi.Services
{
    // public interface IUserCredentials
    // {
    //     string FindMe();
    //     // IEnumerable<ApplicationModule> GetApplicationModule();
    //     // IEnumerable<Menu> GetMenu();
    // }

    public class userCredentials 
    {
        // private readonly IOptions<constrr> _SubdbCon;
        // private readonly dapperQuery _dapper;
        private readonly IOptions<conStr> _dbCon;


        public userCredentials(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        // public static string FindMe (int userID)
        // {
        //     try
        //     {
        //         string cmd = "Select * from view_getcompany where \"userID\" = " + userID + " "; // corrected query string

                
        //         var user = (List<dynamicResponse>)dapperQuery.QryResult<dynamicResponse>(cmd, _dbCon); // assuming _dapper is properly instantiated

        //         if (user.Count > 0)
        //         {
        //             return "Host="+user[0].instanceName+";Database="+user[0].dbName+";Port=5432;Username="+user[0].userName+";Password="+user[0].credentails+"";
        //         }
        //         else
        //         {
        //             return null; // or return an appropriate value when no results are found
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         // Handle exception
        //         return null; // or return an appropriate value when an exception occurs
        //     }
        // }

        public static string FindMe(int userID)
        {
            try
            {
                var cmd = "Select * from view_getcompany where \"userID\" = " + userID + "; "; // corrected query string

                var conStr = "Host=localhost;Database=main-umis;Port=5432;Username=postgres;Password=H!ghR0t@t!0n007";
                // var user = (List<dynamicResponse>)dapperQuery.QryResult<dynamicResponse>(cmd, _dbCon); // assuming _dapper is properly instantiated

                List<dynamicResponse> user = new List<dynamicResponse>(dapperQuery.StrConQry<dynamicResponse>(cmd, conStr));

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

        
    }
}