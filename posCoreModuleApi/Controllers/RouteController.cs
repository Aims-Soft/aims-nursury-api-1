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
    public class RouteController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2;
        public string saveConStr;

        public RouteController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getRoute")]
        public IActionResult getRoute(int businessid,int companyid,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.\"root\" where \"isDeleted\"::int = 0 AND \"businessid\" = " + businessid + " AND \"companyid\" = " + companyid + "";
                var appMenu = _dapperQuery.StrConQry<Route>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveRoute")]
        public IActionResult saveRoute(RouteCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                var found = false;
                var route = "";

                List<Route> appMenuRoute = new List<Route>();
                cmd2 = "select \"rootName\" from root where \"isDeleted\"::int = 0 and \"rootName\" = '" + obj.rootName + "' AND \"businessid\" = " + obj.businessid + " AND \"companyid\" = " + obj.companyid + "";
                appMenuRoute = (List<Route>)_dapperQuery.StrConQry<Route>(cmd2, obj.userID,obj.moduleId);

                if (appMenuRoute.Count > 0)
                    route = appMenuRoute[0].rootName;

                if (obj.rootID == 0)
                {
                    if (route == "")
                    {
                        cmd = "insert into public.root (\"rootName\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\") values ('" + obj.rootName + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + "," + obj.companyid + ")";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.root set \"rootName\" = '" + obj.rootName + "', \"modifiedOn\" = '" + curDate + "', \"modifiedby\" = " + obj.userID + " where \"rootID\" = " + obj.rootID + ";";
                }

                if (found == false)
                {

                     if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                    
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

        [HttpPost("deleteRoute")]
        public IActionResult deleteRoute(RouteCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.root set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedby\" = " + obj.userID + " where \"rootID\" = " + obj.rootID + ";";

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