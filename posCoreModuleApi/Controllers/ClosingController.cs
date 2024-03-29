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

        [HttpGet("getClosingSaleDetail")]
        public IActionResult getClosingSaleDetail(int userID, int moduleId,string curDate,int employeeID)
        {
            try
            {
                cmd = "Select * From \"fun_closingSaleDetails\"('" + curDate + "'::character varying," + employeeID + ")";
                var appMenu = _dapperQuery.StrConQry<ClosingSaleDetail>(cmd,userID,moduleId);
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
                int newCounterDetailID = 0;
                int newAppCounterInfoID = 0;
                var newShiftStartTime = "";
                float totalAmount = 0;
                var response = "";
                
                cmd2 = "update public.tbl_shifts set \"shiftDate\" = '"+obj.shiftDate+"',\"shiftEndTime\" = '"+obj.shiftEndTime+"', \"userID\" = " + obj.counterUserID + ", \"closingBalance\" =" + obj.closingBalance + ", \"difference\" =" + obj.reconsiliation + ", \"remarks\" ='" + obj.remarks + "' ,\"counterID\" = "+obj.counterID+" ,\"createdOn\" = '"+curDate+"', \"createdBy\" = "+obj.userID+" where \"shiftID\" = "+obj.shiftID+"";
               

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

                        cmd3 = "insert into public.\"tbl_counter_detail\" (\"counterDetailID\", \"quantity\", \"shiftID\", \"currencyID\", \"counterFlagID\", \"totalAmount\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + newCounterDetailID + "', '" + item.quantity + "', '" + obj.shiftID + "', '" + item.currencyID + "', 2,'" + item.denomination * item.quantity + "', '" + curDate + "', " + obj.userID + ", B'0')";

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

                if(rowAffected2 > 0)
                {
                // shiftStartTime
                        List<Shift> appMenuStTime = new List<Shift>();
                        cmd = "select \"shiftStartTime\" from tbl_shifts where \"shiftID\" = "+obj.shiftID+"";
                        appMenuStTime = (List<Shift>)_dapperQuery.StrConQry<Shift>(cmd, obj.userID,obj.moduleId);

                        if (appMenuStTime.Count > 0)
                        {
                            newShiftStartTime = appMenuStTime[0].shiftStartTime;
                        }
                    // S

                    List<TotalAmount> appMenuTotalS = new List<TotalAmount>();
                    cmd = "select (Sum(i.\"cashReceived\") - Sum(i.\"change\")) - Sum(i.\"discount\") as \"totalAmount\" "+
                                    "from tbl_shifts s "+
                                    "Inner Join invoice i on i.\"createdBy\" = s.\"userID\" "+
                                    "Inner Join \"invoiceDetail\" id on id.\"invoiceNo\" = i.\"invoiceNo\" "+
                                    "Inner join \"chartOfAccount\" as coa on coa.\"coaID\" = id.\"coaID\" "+
                                    "where i.\"isDeleted\" = B'0' and id.\"isDeleted\" = B'0' and coa.\"coaTitle\" = 'Cash' and i.\"invoiceType\" = 'S' "+
                                    "and s.\"userID\" = "+obj.counterUserID+" and i.\"invoiceDate\" = '"+obj.shiftDate+"' and i.\"invoicetime\" >= '"+newShiftStartTime+"' and i.\"invoicetime\" <= '"+obj.shiftEndTime+"'";
                    appMenuTotalS = (List<TotalAmount>)_dapperQuery.StrConQry<TotalAmount>(cmd, obj.userID,obj.moduleId);

                    if (appMenuTotalS[0].totalAmount > 0)
                    {
                        totalAmount = appMenuTotalS[0].totalAmount;
                        cmd = "select \"appCounterInfoID\" from tbl_app_counter_info ORDER BY \"appCounterInfoID\" DESC LIMIT 1";
                        appMenuTotalS = (List<TotalAmount>)_dapperQuery.StrConQry<TotalAmount>(cmd, obj.userID,obj.moduleId);

                        if (appMenuTotalS.Count > 0)
                        {
                            newAppCounterInfoID = appMenuTotalS[0].appCounterInfoID + 1;
                        }
                        else
                        {
                            newAppCounterInfoID = 1;
                        }

                        cmd4 = "insert into public.\"tbl_app_counter_info\" (\"appCounterInfoID\", \"amount\", \"shiftID\", \"invoiceType\",\"createdOn\", \"createdBy\", \"isDeleted\") values ('" + newAppCounterInfoID + "', '" + totalAmount + "', '" + obj.shiftID + "', 'S', '" + curDate + "', " + obj.userID + ", B'0')";

                            if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                            using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                            {
                                rowAffected2 = con.Execute(cmd4);
                            }
                    }
                    

                // SR

                     List<TotalAmount> appMenuTotalSR = new List<TotalAmount>();
                    cmd = "select (Sum(i.\"cashReceived\") - Sum(i.\"change\")) - Sum(i.\"discount\") as \"totalAmount\" "+
                                    "from tbl_shifts s "+
                                    "Inner Join invoice i on i.\"createdBy\" = s.\"userID\" "+
                                    "Inner Join \"invoiceDetail\" id on id.\"invoiceNo\" = i.\"invoiceNo\" "+
                                    "Inner join \"chartOfAccount\" as coa on coa.\"coaID\" = id.\"coaID\" "+
                                    "where i.\"isDeleted\" = B'0' and id.\"isDeleted\" = B'0' and coa.\"coaTitle\" = 'Cash' and i.\"invoiceType\" = 'SR' "+
                                    "and s.\"userID\" = "+obj.counterUserID+" and i.\"invoiceDate\" = '"+obj.shiftDate+"' and i.\"invoicetime\" >= '"+newShiftStartTime+"' and i.\"invoicetime\" <= '"+obj.shiftEndTime+"'";
                    appMenuTotalSR = (List<TotalAmount>)_dapperQuery.StrConQry<TotalAmount>(cmd, obj.userID,obj.moduleId);

                    if (appMenuTotalSR[0].totalAmount > 0)
                    {
                        totalAmount = appMenuTotalSR[0].totalAmount;
                        cmd = "select \"appCounterInfoID\" from tbl_app_counter_info ORDER BY \"appCounterInfoID\" DESC LIMIT 1";
                        appMenuTotalSR = (List<TotalAmount>)_dapperQuery.StrConQry<TotalAmount>(cmd, obj.userID,obj.moduleId);

                        if (appMenuTotalSR.Count > 0)
                        {
                            newAppCounterInfoID = appMenuTotalSR[0].appCounterInfoID + 1;
                        }
                        else
                        {
                            newAppCounterInfoID = 1;
                        }

                        cmd4 = "insert into public.\"tbl_app_counter_info\" (\"appCounterInfoID\", \"amount\", \"shiftID\", \"invoiceType\",\"createdOn\", \"createdBy\", \"isDeleted\") values ('" + newAppCounterInfoID + "', '" + totalAmount + "', '" + obj.shiftID + "', 'SR', '" + curDate + "', " + obj.userID + ", B'0')";

                            if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                            using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                            {
                                rowAffected2 = con.Execute(cmd4);
                            }
                    }


                // P

                     List<TotalAmount> appMenuTotalP = new List<TotalAmount>();
                    cmd = "select (Sum(i.\"cashReceived\") - Sum(i.\"change\")) - Sum(i.\"discount\") as \"totalAmount\" "+
                                    "from tbl_shifts s "+
                                    "Inner Join invoice i on i.\"createdBy\" = s.\"userID\" "+
                                    "Inner Join \"invoiceDetail\" id on id.\"invoiceNo\" = i.\"invoiceNo\" "+
                                    "Inner join \"chartOfAccount\" as coa on coa.\"coaID\" = id.\"coaID\" "+
                                    "where i.\"isDeleted\" = B'0' and id.\"isDeleted\" = B'0' and coa.\"coaTitle\" = 'Cash' and i.\"invoiceType\" = 'P' "+
                                    "and s.\"userID\" = "+obj.counterUserID+" and i.\"invoiceDate\" = '"+obj.shiftDate+"' and i.\"invoicetime\" >= '"+newShiftStartTime+"' and i.\"invoicetime\" <= '"+obj.shiftEndTime+"'";
                    appMenuTotalP = (List<TotalAmount>)_dapperQuery.StrConQry<TotalAmount>(cmd, obj.userID,obj.moduleId);

                    if (appMenuTotalP[0].totalAmount > 0)
                    {
                        totalAmount = appMenuTotalP[0].totalAmount;

                        cmd = "select \"appCounterInfoID\" from tbl_app_counter_info ORDER BY \"appCounterInfoID\" DESC LIMIT 1";
                        appMenuTotalP = (List<TotalAmount>)_dapperQuery.StrConQry<TotalAmount>(cmd, obj.userID,obj.moduleId);

                        if (appMenuTotalP.Count > 0)
                        {
                            newAppCounterInfoID = appMenuTotalP[0].appCounterInfoID + 1;
                        }
                        else
                        {
                            newAppCounterInfoID = 1;
                        }

                        cmd4 = "insert into public.\"tbl_app_counter_info\" (\"appCounterInfoID\", \"amount\", \"shiftID\", \"invoiceType\",\"createdOn\", \"createdBy\", \"isDeleted\") values ('" + newAppCounterInfoID + "', '" + totalAmount + "', '" + obj.shiftID + "', 'P', '" + curDate + "', " + obj.userID + ", B'0')";

                            if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                            using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                            {
                                rowAffected2 = con.Execute(cmd4);
                            }
                    }                    

                // PR

                     List<TotalAmount> appMenuTotalPR = new List<TotalAmount>();
                    cmd = "select (Sum(i.\"cashReceived\") - Sum(i.\"change\")) - Sum(i.\"discount\") as \"totalAmount\" "+
                                    "from tbl_shifts s "+
                                    "Inner Join invoice i on i.\"createdBy\" = s.\"userID\" "+
                                    "Inner Join \"invoiceDetail\" id on id.\"invoiceNo\" = i.\"invoiceNo\" "+
                                    "Inner join \"chartOfAccount\" as coa on coa.\"coaID\" = id.\"coaID\" "+
                                    "where i.\"isDeleted\" = B'0' and id.\"isDeleted\" = B'0' and coa.\"coaTitle\" = 'Cash' and i.\"invoiceType\" = 'PR' "+
                                    "and s.\"userID\" = "+obj.counterUserID+" and i.\"invoiceDate\" = '"+obj.shiftDate+"' and i.\"invoicetime\" >= '"+newShiftStartTime+"' and i.\"invoicetime\" <= '"+obj.shiftEndTime+"'";
                    appMenuTotalPR = (List<TotalAmount>)_dapperQuery.StrConQry<TotalAmount>(cmd, obj.userID,obj.moduleId);

                    if (appMenuTotalPR[0].totalAmount > 0)
                    {
                        totalAmount = appMenuTotalPR[0].totalAmount;

                        cmd = "select \"appCounterInfoID\" from tbl_app_counter_info ORDER BY \"appCounterInfoID\" DESC LIMIT 1";
                        appMenuTotalPR = (List<TotalAmount>)_dapperQuery.StrConQry<TotalAmount>(cmd, obj.userID,obj.moduleId);

                        if (appMenuTotalPR.Count > 0)
                        {
                            newAppCounterInfoID = appMenuTotalPR[0].appCounterInfoID + 1;
                        }
                        else
                        {
                            newAppCounterInfoID = 1;
                        }

                        cmd4 = "insert into public.\"tbl_app_counter_info\" (\"appCounterInfoID\", \"amount\", \"shiftID\", \"invoiceType\",\"createdOn\", \"createdBy\", \"isDeleted\") values ('" + newAppCounterInfoID + "', '" + totalAmount + "', '" + obj.shiftID + "', 'PR', '" + curDate + "', " + obj.userID + ", B'0')";

                            if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                            using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                            {
                                rowAffected2 = con.Execute(cmd4);
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