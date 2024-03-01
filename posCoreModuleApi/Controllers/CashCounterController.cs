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
    public class CashCounterController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2;
        public string saveConStr;

        public CashCounterController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getCounter")]
        public IActionResult getCounter(int businessid,int companyid,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.\"tbl_counter\" where \"isDeleted\"::int = 0 AND \"businessID\" = " + businessid + " AND \"companyID\" = " + companyid + "";
                var appMenu = _dapperQuery.StrConQry<Counter>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("saveCounter")]
        public IActionResult saveCounter(CounterCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;
                var time = curTime.ToString("HH:mm");
                int rowAffected = 0;
                var response = "";
                var found = false;
                var counter = "";
                var newCounterID = 0;
                List<Counter> appMenuCounterID = new List<Counter>();
                if (obj.spType == "insert")
                {
                    List<Counter> appMenuCounter = new List<Counter>();
                    cmd2 = "select \"counterName\" from tbl_counter where \"isDeleted\"::int = 0 and \"counterName\" = '" + obj.counterName + "' AND \"businessID\" = " + obj.businessid + " AND \"companyID\" = " + obj.companyid + "";
                    appMenuCounter = (List<Counter>)_dapperQuery.StrConQry<Counter>(cmd2, obj.userID,obj.moduleId);

                    if (appMenuCounter.Count > 0)
                        counter = appMenuCounter[0].counterName;

                    cmd2 = "select (max(\"counterID\")+1) as \"counterID\" from \"tbl_counter\" ";
                    appMenuCounterID = (List<Counter>)_dapperQuery.StrConQry<Counter>(cmd2, obj.userID,obj.moduleId);

                    if (appMenuCounterID.Count > 0)
                    {
                        newCounterID = appMenuCounterID[0].counterID;
                    }

                    // if(menuCounterID.Count == 0)
                    //     {
                    //         newCounterID = 1;
                    //     }else{
                    //         newCounterID = menuCounterID[0].counterID+1;
                    //     }

                    if (obj.counterID == 0)
                    {
                        if (counter == "")
                        {
                            cmd = "insert into public.tbl_counter (\"counterID\",\"counterName\",\"counterNo\",\"grassAmount\",\"createdOn\",\"createdBy\",\"isDeleted\",\"businessID\",\"companyID\") values (" + newCounterID + ", '" + obj.counterName + "'," + obj.counterNo + "," + obj.grassAmount + ", '" + curDate + "'," + obj.userID + ",B'0'," + obj.businessid + "," + obj.companyid + ")";
                        }
                        else
                        {
                            found = true;
                        }
                    }
                    else
                    {
                        cmd = "update public.tbl_counter set \"counterName\" = '" + obj.counterName + "', \"counterNo\" = " + obj.counterNo + ",\"grassAmount\" = " + obj.grassAmount + ", \"modifiedOn\" = '" + curDate + "',\"modifiedBy\" = " + obj.userID + " where \"counterID\" = " + obj.counterID + ";";
                    }   
                }
                else if (obj.spType == "delete")
                {
                    if (obj.counterID != 0)
                    {
                        cmd = "update public.tbl_counter set \"isDeleted\" = B'0',\"deletedOn\" = '" + curDate + "',\"deletedBy\" = " + obj.userID + " where \"counterID\" = " + obj.counterID + ";";
                    }
                    else
                    {
                        found = true;
                    }
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
        
    }
}