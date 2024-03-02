using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using posCoreModuleApi.Services;
using Microsoft.Extensions.Options;
using posCoreModuleApi.Configuration;
using posCoreModuleApi.Entities;
using Dapper;
using System.Data;
using Npgsql;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace posCoreModuleApi.dto.response
{
    [ApiController]
    [Route("[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2,cmd3,cmd4;
        public string saveConStr;

        public PackageController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getPackages")]
        public IActionResult getPackages(int businessID,int companyID, int userID, int moduleId)
        {
            try
            {
                cmd = "Select * From view_package Where \"businessID\" = 3 and \"branchID\" = 4 and \"companyID\" = 1 Order by \"packageTitle\" ASC";
                var appMenu = _dapperQuery.StrConQry<PackageDetail>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("savePackage")]
        public IActionResult savePackage(PackageCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;
                var time = curTime.ToString("HH:mm");
                int rowAffected = 0;
                int rowAffected2 = 0;
                int newPackageID = 0;
                int newPackageDetailID = 0;
                var response = "";
                var found = false;
                var packageTitle = "";

                List<Package> appMenuShift = new List<Package>();
                cmd = "select \"packageID\" from tbl_package ORDER BY \"packageID\" DESC LIMIT 1";
                appMenuShift = (List<Package>)_dapperQuery.StrConQry<Package>(cmd, obj.userID,obj.moduleId);

                if (appMenuShift.Count > 0)
                {
                    newPackageID = appMenuShift[0].packageID + 1;
                }
                else
                {
                    newPackageID = 1;
                }
                List<Package> appMenuCounter = new List<Package>();
                cmd2 = "select \"packageTitle\" from tbl_package where \"isDeleted\"::int = 0 and \"packageTitle\" = '" + obj.packageTitle + "' AND \"businessID\" = " + obj.businessID + " AND \"companyID\" = " + obj.companyID + "";
                appMenuCounter = (List<Package>)_dapperQuery.StrConQry<Package>(cmd2, obj.userID,obj.moduleId);

                if (appMenuCounter.Count > 0)
                    packageTitle = appMenuCounter[0].packageTitle;
                    
                if (packageTitle == "")
                {
                    cmd2 = "insert into public.tbl_package (\"packageID\", \"packageTitle\", \"packageDate\", \"businessID\", \"companyID\",\"branchID\" ,\"createdOn\", \"createdBy\", \"isDeleted\") values (" + newPackageID + ", '" + obj.packageTitle + "', '" + obj.packageDate + "', " + obj.businessID + ", " + obj.companyID + ", "+ obj.branchID +" ,'" + curDate + "', " + obj.userID + ", B'0')";
                }
                else
                {
                    found = true;
                }
                
                if(obj.userID != 0 && obj.moduleId !=0)
                {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                }

                using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                {
                    rowAffected = con.Execute(cmd2);
                }

                if (rowAffected > 0)
                {
                    var invObject = JsonConvert.DeserializeObject<List<Product>>(obj.json);
                    foreach (var item in invObject)
                    {
                        List<Package> appMenuDetail = new List<Package>();
                        cmd = "select \"packageDetailID\" from tbl_package_details ORDER BY \"packageDetailID\" DESC LIMIT 1";
                        appMenuDetail = (List<Package>)_dapperQuery.StrConQry<Package>(cmd, obj.userID,obj.moduleId);

                        if (appMenuDetail.Count > 0)
                        {
                            newPackageDetailID = appMenuDetail[0].packageDetailID + 1;
                        }
                        else
                        {
                            newPackageDetailID = 1;
                        }

                        cmd3 = "insert into public.\"tbl_package_details\" (\"packageDetailID\", \"productID\", \"packageID\", \"businessID\", \"companyID\", \"branchID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + newPackageDetailID + "', " + item.productID + ", " + newPackageID + "," + obj.businessID + "," + obj.companyID + "," + obj.branchID + ", '" + curDate + "', " + obj.userID + ", B'0')";

                        if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected2 = con.Execute(cmd3);
                        }
                    }
                }

                if (rowAffected > 0 && rowAffected2 > 0)
                {
                    response = "Success";
                }
                else
                {
                    response = "Server Issue";
                }

                return Ok(new { message = response});
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }        
    }
}