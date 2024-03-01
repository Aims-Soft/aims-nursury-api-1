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

namespace posCoreModuleApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClosingController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2,cmd3,cmd4;
        public string saveConStr;

        public ClosingController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getClosingReconciliation")]
        public IActionResult getClosingReconciliation(int userID, int moduleId,string curDate,int counterID,int employeeID)
        {
            try
            {
                cmd = "Select * From \"fun_closingReconciliation\"('" + curDate + "'::character varying," + counterID + "," + employeeID + ")";
                var appMenu = _dapperQuery.StrConQry<ClosingReconciliation>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("saveClosingBalance")]
        public IActionResult saveClosingBalance(ClosingCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                int newShiftID = 0;
                int newCounterDetailID = 0;
                var response = "";

                // List<Shift> appMenuShift = new List<Shift>();
                // cmd = "select \"shiftID\" from tbl_shifts ORDER BY \"shiftID\" DESC LIMIT 1";
                // appMenuShift = (List<Shift>)_dapperQuery.StrConQry<Shift>(cmd, obj.userID,obj.moduleId);

                // if (appMenuShift.Count > 0)
                // {
                //     newShiftID = appMenuShift[0].shiftID + 1;
                // }
                // else
                // {
                //     newShiftID = 1;
                // }
                
                cmd2 = "update public.tbl_shifts set \"shiftDate\" = '"+obj.shiftDate+"' , \"shiftEndTime\" = '"+obj.shiftEndTime+"', \"userID\" = " + obj.counterUserID + ", \"closingBalance\" =" + obj.closingBalance + " ,\"counterID\" = "+obj.counterID+" ,\"createdOn\" = '"+curDate+"', \"createdBy\" = "+obj.userID+" where \"shiftID\" = "+obj.shiftID+"";
               

                if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }

                using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                {
                    rowAffected = con.Execute(cmd2);
                }

                //confirmation of data saved in CounterDetail
                if (rowAffected > 0)
                {

                   
                    //convert string to json data to insert in invoice detail table
                    var invObject = JsonConvert.DeserializeObject<List<ClosingJsonCreation>>(obj.json);


                    //saving json data one by one in invoice detail table
                    foreach (var item in invObject)
                    {

                        List<CounterDetail> appMenuDetail = new List<CounterDetail>();
                        cmd = "select \"counterDetailID\" from tbl_counter_detail ORDER BY \"counterDetailID\" DESC LIMIT 1";
                        appMenuDetail = (List<CounterDetail>)_dapperQuery.StrConQry<CounterDetail>(cmd, obj.userID,obj.moduleId);

                        if (appMenuDetail.Count > 0)
                        {
                            newCounterDetailID = appMenuDetail[0].counterDetailID + 1;
                        }
                        else
                        {
                            newCounterDetailID = 1;
                        }

                        cmd3 = "insert into public.\"tbl_counter_detail\" (\"counterDetailID\", \"quantity\", \"shiftID\", \"currencyID\", \"counterFlagID\", \"totalAmount\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + newCounterDetailID + "', '" + item.quantity + "', '" + obj.shiftID + "', '" + item.currencyID + "', 1,'" + item.denomination * item.quantity + "', '" + curDate + "', " + obj.userID + ", B'0')";

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