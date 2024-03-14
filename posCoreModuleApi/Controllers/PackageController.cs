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
                cmd = "Select Distinct \"packageID\",\"packageTitle\",\"barcode\",\"packageDate\" From view_package Where \"businessID\" = " + businessID + " and \"companyID\" = " + companyID + " Order by \"packageTitle\" ASC";
                var appMenu = _dapperQuery.StrConQry<PackageDetail>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getPackageDetails")]
        public IActionResult getPackageDetails(int businessID,int companyID, int userID, int moduleId,int packageID)
        {
            try
            {
                cmd = "Select Distinct \"productID\",\"productName\",\"productNameUrdu\",\"salePrice\",\"productBarcode\" From view_package Where \"businessID\" = " + businessID + " and \"companyID\" = " + companyID + " and \"packageID\" = " + packageID + " Order by \"productName\" ASC";
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
                int rowAffected3 = 0;
                int newPackageID = 0;
                int newPackageDetailID = 0;
                var response = "";
                var found = false;
                var check = false;
                var packageTitle = "";
                if (obj.packageID == 0)
                {
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
                        cmd2 = "insert into public.tbl_package (\"packageID\", \"packageTitle\", \"barcode\", \"packageDate\", \"businessID\", \"companyID\",\"branchID\" ,\"createdOn\", \"createdBy\", \"isDeleted\") values (" + newPackageID + ", '" + obj.packageTitle + "', '" + obj.barcode + "', '" + obj.packageDate + "', " + obj.businessID + ", " + obj.companyID + ", "+ obj.branchID +" ,'" + curDate + "', " + obj.userID + ", B'0')";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    List<Package> appMenuCounter = new List<Package>();
                    cmd2 = "select \"packageTitle\" from tbl_package where \"isDeleted\"::int = 0 and \"packageTitle\" = '" + obj.packageTitle + "' AND \"businessID\" = " + obj.businessID + " AND \"companyID\" = " + obj.companyID + " AND \"packageID\" != " + obj.packageID + "";
                    appMenuCounter = (List<Package>)_dapperQuery.StrConQry<Package>(cmd2, obj.userID,obj.moduleId);

                    if (appMenuCounter.Count > 0)
                        packageTitle = appMenuCounter[0].packageTitle;
                        
                    if (packageTitle == "")
                    {
                        cmd2 = "UPDATE public.tbl_package "+
                                " SET \"packageTitle\" = '" + obj.packageTitle + "', \"packageDate\"= '" + curDate + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + ", barcode = '" + obj.barcode + "' "+
                                " WHERE \"packageID\" = " + obj.packageID + ";";
                    }
                    else
                    {
                        found = true;
                    }   
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
                    if (newPackageID == 0)
                    {
                        newPackageID = obj.packageID;    
                    }
                    
                    cmd4 = "UPDATE public.tbl_package_details "+
                        " SET \"isDeleted\" = B'1' "+
                        " WHERE \"packageID\" = "+ obj.packageID +";";

                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected3 = con.Execute(cmd4);
                    }

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

                        List<Package> checkIfExist = new List<Package>();
                        cmd = "select \"packageDetailID\" from tbl_package_details Where \"packageID\" = " + obj.packageID + " AND \"productID\" = " + item.productID + " ";
                        checkIfExist = (List<Package>)_dapperQuery.StrConQry<Package>(cmd, obj.userID,obj.moduleId);
                        if (checkIfExist.Count > 0)
                        {
                            check = true;
                        }
                        else
                        {
                            check = false;
                        }
                        if(check == false)
                        {
                            cmd3 = "insert into public.\"tbl_package_details\" (\"packageDetailID\", \"productID\", \"packageID\", \"businessID\", \"companyID\", \"branchID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + newPackageDetailID + "', " + item.productID + ", " + newPackageID + "," + obj.businessID + "," + obj.companyID + "," + obj.branchID + ", '" + curDate + "', " + obj.userID + ", B'0')";
                        }
                        else
                        {
                            cmd3 = "UPDATE public.tbl_package_details "+
                            " SET \"isDeleted\" = B'0' "+
                            " Where \"packageID\" = " + obj.packageID + " AND \"productID\" = " + item.productID + "";
                        }
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
                    if (found == true)
                    {
                        response = "Package Already Exist.";    
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                }

                return Ok(new { message = response});
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }        

        [HttpPost("deletePackage")]
        public IActionResult deletePackage(PackageCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                var response = "";

                cmd = "update tbl_package set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"packageID\" = " + obj.packageID + ";";
                if(obj.userID != 0 && obj.moduleId !=0)
                {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                    rowAffected = con.Execute(cmd);
                    }
                }

                if (rowAffected > 0)
                {
                    response = "Success";
                }
                else
                {
                    response = "Invalid Input Error";
                }
                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
    }
}