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
    public class ChartOfAccountController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2,cmd3,cmd4;
        public string saveConStr;
        // private string dbCon2 = "Host=localhost;Database=main-fmis;Port=5432;Username=postgres;Password=H!ghR0t@t!0n007";

        public ChartOfAccountController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon; 
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getCOAType")]
        public IActionResult getCOAType(int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.\"coaType\"";
        
                var appMenu = _dapperQuery.StrConQry<COAType>(cmd,userID,moduleId);
                // var appMenu = dapperQuery.Qry<Bank>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getCOA")]
        public IActionResult getCOA(int companyID, int businessID,int userID, int moduleId)
        {
            try
            {
                
                cmd = "select * from \"view_chartofAccount\" where businessid is null or (companyid = " + companyID + " AND businessid = " + businessID + ")";
                var appMenu = _dapperQuery.StrConQry<COA>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getCOASubTypeWise")]
        public IActionResult getCOASubTypeWise(int companyID, int businessID,int userID, int moduleId)
        {
            try
            {
                
                cmd = "select * from \"chartOfAccount\" where  companyid = " + companyID + " AND businessid = " + businessID + " AND \"isDeleted\"::int = 0 and subtype = 'bank'";
                var appMenu = _dapperQuery.StrConQry<COA>(cmd,userID,moduleId);
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
                var coaID = 0;

                List<COA> appMenuTitle = new List<COA>();
                cmd2 = "select \"coaTitle\" from \"chartOfAccount\" where \"isDeleted\"::int = 0 and \"coaTitle\" = '" + obj.coaTitle + "'";
                appMenuTitle = (List<COA>)_dapperQuery.StrConQry<COA>(cmd2, obj.userID,obj.moduleId);
                // appMenuTitle = (List<COA>)dapperQuery.QryResult<COA>(cmd2, _dbCon);

                if (appMenuTitle.Count > 0)
                    title = appMenuTitle[0].coaTitle;

                if (obj.coaID == 0)
                {
                    if (title == "")
                    {
                        cmd4 = "select \"coaID\" from public.\"chartOfAccount\" order by \"coaID\" desc limit 1";
                        appMenuTitle = (List<COA>)_dapperQuery.StrConQry<COA>(cmd4, obj.userID,obj.moduleId);

                        if (appMenuTitle.Count > 0)
                        {
                            coaID = appMenuTitle[0].coaID + 1;
                        }
                        else
                        {
                            coaID = 1;
                        }
                        cmd = "insert into public.\"chartOfAccount\" (\"coaID\",\"coaTypeID\", \"coaTitle\", \"coaAlias\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + coaID + "','" + obj.coaTypeID + "', '" + obj.coaTitle + "', '" + obj.coaTitle + "', '" + curDate + "', " + obj.userID + ", B'0')";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.\"chartOfAccount\" set \"coaTypeID\" = '" + obj.coaTypeID + "', \"coaTitle\" = '" + obj.coaTitle + "', \"coaAlias\" = '" + obj.coaTitle + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"coaID\" = " + obj.coaID + ";";
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