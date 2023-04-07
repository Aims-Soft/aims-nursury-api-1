using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FMISModuleApi.Services;
using Microsoft.Extensions.Options;
using FMISModuleApi.Configuration;
using FMISModuleApi.Entities;
using Dapper;
using System.Data;
using Npgsql;
using System.Collections.Generic;

namespace FMISModuleApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ChartOfAccountController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2;

        public ChartOfAccountController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getCOAType")]
        public IActionResult getCOAType()
        {
            try
            {
                cmd = "select * from public.\"coaType\"";
                var appMenu = dapperQuery.Qry<COAType>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getCOA")]
        public IActionResult getCOA(int companyID, int businessID)
        {
            try
            {
                if(companyID == 0 && businessID == 0){
                    cmd = "select * from \"view_chartofAccount\"";
                }else{
                    cmd = "select * from \"view_chartofAccount\" where companyid = " + companyID + " AND businessid = " + businessID + "";
                }
                var appMenu = dapperQuery.Qry<COA>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getCOASubTypeWise")]
        public IActionResult getCOASubTypeWise(int companyID, int businessID)
        {
            try
            {
                if(companyID == 0 && businessID == 0){
                    cmd = "select * from \"chartOfAccount\" where \"isDeleted\"::int = 0 and subtype = 'bank'";
                }else{
                    cmd = "select * from \"chartOfAccount\" where  companyid = " + companyID + " AND businessid = " + businessID + " AND \"isDeleted\"::int = 0 and subtype = 'bank'";
                }
                var appMenu = dapperQuery.Qry<COA>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveCOA")]
        public IActionResult saveCOA(COACreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                var found = false;
                var title = "";

                List<COA> appMenuTitle = new List<COA>();
                cmd2 = "select \"coaTitle\" from \"chartOfAccount\" where \"isDeleted\"::int = 0 and \"coaTitle\" = '" + obj.coaTitle + "' AND companyid = " + obj.companyid + " AND businessid = " + obj.businessid + "";
                appMenuTitle = (List<COA>)dapperQuery.QryResult<COA>(cmd2, _dbCon);

                if (appMenuTitle.Count > 0)
                    title = appMenuTitle[0].coaTitle;

                if (obj.coaID == 0)
                {
                    if (title == "")
                    {
                        cmd = "insert into public.\"chartOfAccount\" (\"coaTypeID\", \"coaTitle\", \"coaAlias\", companyid, businessid, \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.coaTypeID + "', '" + obj.coaTitle + "', '" + obj.coaTitle + "', '" + obj.companyid + "', '" + obj.businessid + "', '" + curDate + "', " + obj.userID + ", B'0')";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.\"chartOfAccount\" set \"coaTypeID\" = '" + obj.coaTypeID + "', \"coaTitle\" = '" + obj.coaTitle + "', \"coaAlias\" = '" + obj.coaTitle + "', \"companyid\" = '" + obj.companyid + "', \"businessid\" = '" + obj.businessid + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"coaID\" = " + obj.coaID + ";";
                }

                if (found == false)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
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

        [HttpPost("deleteCOA")]
        public IActionResult deleteCOA(COACreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.\"chartOfAccount\" set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"coaID\" = " + obj.coaID + ";";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
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