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
    public class BankController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2,cmd3,cmd4;
        public string saveConStr;
        // private string dbCon2 = "Host=localhost;Database=main-fmis;Port=5432;Username=postgres;Password=H!ghR0t@t!0n007";

        public BankController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getBank")]
        public IActionResult getBank(int businessID, int companyID,int userID, int moduleId)
        {
            try
            {
                if(businessID == 0 && companyID == 0){
                cmd = "SELECT * FROM public.bank where \"isDeleted\"::int = 0 ORDER BY \"bankID\" DESC";
                }else{
                cmd = "SELECT * FROM public.bank where \"isDeleted\"::int = 0 and businessid = " + businessID + "  and companyid = " + companyID + " ORDER BY \"bankID\" DESC";
                }

                var appMenu = _dapperQuery.StrConQry<Bank>(cmd,userID,moduleId);
                // var appMenu = dapperQuery.Qry<Bank>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("saveBank")]
        public IActionResult saveBank(BankCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");
                List<Bank> appMenucoaID = new List<Bank>();

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                var found = false;
                var bank = "";

                List<Bank> appMenuBank = new List<Bank>();
                cmd2 = "select \"bankName\" from bank where \"isDeleted\"::int = 0 and \"accountNo\"='" + obj.accountNo + "'";
                appMenuBank = (List<Bank>)_dapperQuery.StrConQry<Bank>(cmd2, obj.userID,obj.moduleId);
                // appMenuBank = (List<Bank>)dapperQuery.QryResult<Bank>(cmd2, _dbCon);

                if (appMenuBank.Count > 0)
                    bank = appMenuBank[0].bankName;

                if (obj.bankID == 0)
                {
                    if (bank == "")
                    {
                        // increase primary key of coaID

                        cmd4 = "select \"coaID\" from public.\"chartOfAccount\" order by \"coaID\" desc limit 1";
                        appMenucoaID = (List<Bank>)_dapperQuery.StrConQry<Bank>(cmd4, obj.userID,obj.moduleId);

                        var coaID = 0;
                        if (appMenucoaID.Count > 0)
                        {
                            coaID = appMenucoaID[0].coaID + 1;
                        }
                        else
                        {
                            coaID = 1;
                        }
                       
                        cmd2 = "insert into public.\"chartOfAccount\" (\"coaID\",\"coaTypeID\", \"coaTitle\", \"subtype\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\") values ('" + coaID + "',1, '" + obj.accountTitle + "', 'bank', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + "," + obj.companyid + ")";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.\"bank\" set \"bankName\" = '" + obj.bankName + "', \"branchCode\" = '" + obj.branchCode + "', \"branchAddress\" = '" + obj.branchAddress + "', \"accountNo\" = '" + obj.accountNo + "', \"accountTitle\" = '" + obj.accountTitle + "', \"description\" = '" + obj.description + "', \"type\" = '" + obj.type + "', \"branchname\" = '" + obj.branchname + "', \"amount\" = '" + obj.amount + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"bankID\" = " + obj.bankID + ";";
                    cmd2 = "update public.\"chartOfAccount\" set \"coaTitle\" = '" + obj.accountTitle + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"coaID\" = " + obj.coaID + ";";
                }

                if (found == false)
                {
                    if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                    
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected = con.Execute(cmd2);
                    }
                }

                if (rowAffected > 0)
                {

                    if (obj.bankID == 0)
                    {

                        cmd2 = "SELECT \"coaID\" FROM public.\"chartOfAccount\" order by \"coaID\" desc limit 1";

                        appMenucoaID = (List<Bank>)_dapperQuery.StrConQry<Bank>(cmd2, obj.userID,obj.moduleId);
                        // appMenucoaID = (List<Bank>)dapperQuery.StrConQry<Bank>(cmd2, dbCon2);
                        var coaID = appMenucoaID[0].coaID;

                        cmd = "insert into public.\"bank\" (\"bankName\", \"branchCode\", \"branchAddress\", \"accountNo\", \"accountTitle\", \"description\", \"type\", \"branchname\", \"amount\", \"coaid\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\",\"branchID\") values ('" + obj.bankName + "', '" + obj.branchCode + "', '" + obj.branchAddress + "', '" + obj.accountNo + "', '" + obj.accountTitle + "', '" + obj.description + "', '" + obj.type + "', '" + obj.branchname + "', '" + obj.amount + "', '" + coaID + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + "," + obj.companyid + "," + obj.branchID + ")";

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


                    }

                   

                    // using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    // {
                    //     rowAffected2 = con.Execute(cmd);
                    // }


                    if (rowAffected > 0)
                    {
                        response = "Success";
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                }
                else
                {
                    if (found == true)
                    {
                        response = "Account no already exist";
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

        [HttpPost("deleteBank")]
        public IActionResult deleteBank(BankCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";

                cmd = "update public.\"bank\" set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"bankID\" = " + obj.bankID + ";";
                cmd2 = "update public.\"chartOfAccount\" set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"coaID\" = " + obj.coaID + ";";

                if(obj.userID != 0 && obj.moduleId !=0)
                    {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                        rowAffected = con.Execute(cmd);
                        }
                    }

                if(obj.userID != 0 && obj.moduleId !=0)
                    {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                        rowAffected2 = con.Execute(cmd);
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