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
    public class DesignationController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2;
        private string subconStr;

        public DesignationController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getDesignation")]
        public IActionResult getDesignation(int businessID, int companyID,int userID)
        {
            try
            {
                if(companyID == 0 && businessID == 0){
                    cmd = "SELECT * FROM public.designation where \"isDeleted\"::int = 0";
                }else{
                cmd = "SELECT * FROM public.designation where \"isDeleted\"::int = 0 AND \"businessid\" = " + businessID + " AND \"companyid\" = " + companyID + "";
                }
                subconStr = userCredentials.FindMe(userID);
                var appMenu = dapperQuery.StrConQry<Designation>(cmd, subconStr);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }


        [HttpPost("saveDesignation")]
        public IActionResult saveDesignation(DesignationCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                var found = false;
                var designation = "";

                List<Designation> appMenuDesignation = new List<Designation>();
                cmd2 = "select \"desginationName\" from designation where \"isDeleted\"::int = 0 and \"desginationName\" = '" + obj.designationName + "' and \"businessid\" = " + obj.businessid + " AND \"companyid\" = " + obj.companyid + "";
                subconStr = userCredentials.FindMe(obj.userID);
                appMenuDesignation = (List<Designation>)dapperQuery.StrConQry<Designation>(cmd2, subconStr);

                if (appMenuDesignation.Count > 0)
                    designation = appMenuDesignation[0].desginationName;

                if (obj.designationID == 0)
                {

                    if (designation == "")
                    {
                        cmd = "insert into public.designation (\"desginationName\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\") values ('" + obj.designationName + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + ", " + obj.companyid + ")";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.designation set \"desginationName\" = '" + obj.designationName + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"designationID\" = " + obj.designationID + ";";
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

        [HttpPost("deleteDesignation")]
        public IActionResult deleteDesignation(DesignationCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.designation set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"designationID\" = " + obj.designationID + ";";

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