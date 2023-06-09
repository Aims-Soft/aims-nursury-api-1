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

namespace posCoreModuleApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2;
        private string subconStr;

        public BrandController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getBrand")]
        public IActionResult getBrand(int businessid,int companyid, int userID)
        {
            try
            {
                cmd = "select * from public.\"brand\" where \"isDeleted\"::int = 0 AND \"businessid\" = " + businessid + " AND \"companyid\" = " + companyid + "";
                subconStr = userCredentials.FindMe(userID);
                var appMenu = dapperQuery.StrConQry<Brand>(cmd, subconStr);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveBrand")]
        public IActionResult saveBrand(BrandCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                var found = false;
                var brand = "";

                List<Brand> appMenuBrand = new List<Brand>();
                cmd2 = "select \"brandName\" from brand where \"isDeleted\"::int = 0 and \"brandName\" = '" + obj.brandName + "' AND \"businessid\" = " + obj.businessid + " AND \"companyid\" = " + obj.companyid + "";
                subconStr = userCredentials.FindMe(obj.userID);
                appMenuBrand = (List<Brand>)dapperQuery.StrConQry<Brand>(cmd2, subconStr);

                if (appMenuBrand.Count > 0)
                    brand = appMenuBrand[0].brandName;

                if (obj.brandID == 0)
                {
                    if (brand == "")
                    {
                        cmd = "insert into public.brand (\"brandName\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\") values ('" + obj.brandName + "', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + "," + obj.companyid + ")";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.brand set \"brandName\" = '" + obj.brandName + "', \"description\" = '" + obj.description + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"brandID\" = " + obj.brandID + ";";
                }

                if (found == false)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(subconStr))
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
                    if (found == true)
                    {
                        response = "Record already exist";
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                }

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("deleteBrand")]
        public IActionResult deleteBrand(BrandCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.brand set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"brandID\" = " + obj.brandID + ";";

                subconStr = userCredentials.FindMe(obj.userID);
                using (NpgsqlConnection con = new NpgsqlConnection(subconStr))
                {
                    rowAffected = con.Execute(cmd);
                }

                if (rowAffected > 0)
                {
                    response = "Success";
                }
                else
                {
                    response = "Server Issue";
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