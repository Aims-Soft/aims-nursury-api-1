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
    public class PartyController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        public string cmd, cmd2;
        public string saveConStr;

        public PartyController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getParty")]
        public IActionResult getParty(int branchID,int companyID,int userID, int moduleId)
        {
            try
            {
                if (branchID != 0 && companyID == 0)
                {
                    cmd = "SELECT * FROM view_party where \"branchID\" = " + branchID + " order by \"partyID\" desc";
                }
                else
                {
                    cmd = "SELECT * FROM view_party where \"branchID\" = " + branchID + " and \"companyid\" = " + companyID + " order by \"partyID\" desc";
                }
                var appMenu = _dapperQuery.StrConQry<Party>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getAllParties")]
        public IActionResult getAllParties(int branchID,int companyID,int userID, int moduleId)
        {
            try
            {
                if (branchID != 0 && companyID == 0)
                {
                    cmd = "select * from public.party where \"isDeleted\"::int = 0 AND \"branchID\" = " + branchID + "  order by \"partyID\" desc";
                }
                else
                {
                    cmd = "select * from public.party where \"isDeleted\"::int = 0 AND \"branchID\" = " + branchID + " and \"companyid\" = " + companyID + "";
                }
                
                var appMenu = _dapperQuery.StrConQry<Party>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("saveParty")]
        public IActionResult saveParty(PartyCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                var found = false;
                var cnic = "";

                List<Party> appMenuParty = new List<Party>();
                cmd2 = "select cnic from party where \"isDeleted\"::int = 0 AND cnic = '" + obj.cnic + "' AND (\"type\" = 'supplier' OR \"type\" = 'customer')";
                appMenuParty = (List<Party>)_dapperQuery.StrConQry<Party>(cmd2, obj.userID,obj.moduleId);

                if (appMenuParty.Count > 0)
                    cnic = appMenuParty[0].cnic;

                if (obj.partyID == 0)
                {
                    if (cnic == "")
                    {
                        cmd = "insert into public.\"party\" (\"cityID\", \"partyName\", \"partyNameUrdu\", \"address\", \"addressUrdu\", \"phone\", \"mobile\", \"type\", \"description\", \"cnic\", \"focalperson\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\",\"branchID\") values ('" + obj.cityID + "', '" + obj.partyName + "', '" + obj.partyNameUrdu + "', '" + obj.address + "', '" + obj.addressUrdu + "', '" + obj.phone + "', '" + obj.mobile + "', '" + obj.type + "', '" + obj.description + "', '" + obj.cnic + "', '" + obj.focalPerson + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + "," + obj.companyid + "," + obj.branchID + ")";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.\"party\" set \"cityID\" = '" + obj.cityID + "', \"partyName\" = '" + obj.partyName + "', \"partyNameUrdu\" = '" + obj.partyNameUrdu + "', \"address\" = '" + obj.address + "', \"addressUrdu\" = '" + obj.addressUrdu + "', \"phone\" = '" + obj.phone + "', \"mobile\" = '" + obj.mobile + "', \"type\" = '" + obj.type + "', \"description\" = '" + obj.description + "', \"cnic\" = '" + obj.cnic + "', \"focalperson\" = '" + obj.focalPerson + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"partyID\" = " + obj.partyID + ";";
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
                        response = "CNIC already exist";
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

        [HttpPost("deleteParty")]
        public IActionResult deleteParty(PartyCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.\"party\" set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"partyID\" = " + obj.partyID + ";";
                    
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

        [HttpPost("saveCustomerSale")]
        public IActionResult saveCustomerSale(CustomerCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                var found = false;
                var cnic = "";

                List<Customers> appMenuParty = new List<Customers>();                 
                cmd2 = "select cnic from party where \"isDeleted\"::int = 0 AND cnic = '" + obj.cnic + "' AND (\"type\" = 'customer')";
                appMenuParty = (List<Customers>)_dapperQuery.StrConQry<Customers>(cmd2, obj.userID,obj.moduleId);

                if (appMenuParty.Count > 0)
                cnic = appMenuParty[0].cnic;

                if (obj.partyID == 0)
                {
                    if (cnic == "")
                    {
                        cmd = "insert into public.\"party\" (\"partyName\", \"mobile\", \"type\", \"cnic\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\",\"branchID\") values ('" + obj.partyName + "', '" + obj.mobile + "', 'customer', '" + obj.cnic + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + "," + obj.companyid + ", "+obj.branchID+" )";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.\"party\" set \"partyName\" = '" + obj.partyName + "', \"mobile\" = '" + obj.mobile + "', \"cnic\" = '" + obj.cnic + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"partyID\" = " + obj.partyID + ";";
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
                        response = "CNIC already exist";
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

        [HttpGet("getAllCustomer")]
        public IActionResult getAllCustomer(int branchID,int userID, int moduleId)
        {
            try
            {
            
                    cmd = "select * from public.party where \"isDeleted\"::int = 0 AND \"branchID\" = " + branchID + " and \"type\" = 'customer' order by \"partyID\" desc";
                
                var appMenu = _dapperQuery.StrConQry<Customers>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

    }
}