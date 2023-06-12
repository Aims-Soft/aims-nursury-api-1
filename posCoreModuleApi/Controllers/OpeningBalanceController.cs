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

namespace posCoreModuleApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class OpeningBalanceController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2, cmd3;
        public string saveConStr;

        public OpeningBalanceController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getOpeningBalance")]
        public IActionResult getOpeningBalance(int branchID,int businessID,int companyID,int userID, int moduleId)
        {
            try
            {  
                if (businessID == 0 && companyID == 0 && branchID == 0)
                {
                    cmd = "SELECT * FROM \"view_openingBalance\" order by \"invoiceNo\" desc";
                } 
                else
                {
                    cmd = "SELECT * FROM \"view_openingBalance\" where \"businessid\" = " + businessID + " AND \"companyid\" = " + companyID + " AND \"branchid\" = " + branchID + " order by \"invoiceNo\" desc";
                }
                var appMenu = _dapperQuery.StrConQry<OpeningBalance>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }
        
        [HttpGet("getOpeningBalanceProduct")]
        public IActionResult getOpeningBalanceProduct(int categoryID,int branchID,int businessID,int companyID,int userID, int moduleId)
        {
            try
            {
                if (businessID == 0 && companyID == 0 && branchID == 0)
                {
                    cmd = "SELECT * FROM \"view_openingBalance_product\" where \"categoryID\" = " + categoryID + " order by \"productID\" desc";
                }
                else
                {
                    cmd = "SELECT * FROM \"view_openingBalance_product\" where \"categoryID\" = " + categoryID + " AND \"branchid\" = " + branchID + " AND \"businessid\" = " + businessID + " AND \"companyid\" = " + companyID + " order by \"productID\" desc";    
                }
                
                var appMenu = _dapperQuery.StrConQry<OpeningBalanceProduct>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveBalance")]
        public IActionResult saveBalance(OpeningBalanceCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                // DateTime curTime = DateTime.Now;
                var time = Convert.ToDateTime(obj.invoiceDate).ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                List<Invoice> appMenuInvoice = new List<Invoice>();

                if(obj.invoiceNo == 0){
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"invoiceType\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('"+ obj.invoiceDate +"', '"+ time +"', 'OB', '"+ curDate +"', "+ obj.userID +", B'0'," + obj.branchid + ", " + obj.businessid + "," + obj.companyid + ")";

                    if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                    
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected = con.Execute(cmd);
                    }   

                    //confirmation of data saved in invoice
                    if(rowAffected > 0){

                        //getting last saved invoice no
                        cmd2 = "SELECT \"invoiceNo\" FROM public.invoice order by \"invoiceNo\" desc limit 1";
                        appMenuInvoice = (List<Invoice>)_dapperQuery.StrConQry<Invoice>(cmd2, obj.userID,obj.moduleId);

                        var invoiceNo = appMenuInvoice[0].invoiceNo;


                            cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"qty\", \"costPrice\", \"salePrice\", \"debit\", \"credit\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('"+ invoiceNo +"', '"+ obj.productID +"', '"+ obj.qty +"', '"+ obj.costPrice +"', '"+ obj.salePrice +"', '"+ obj.debit +"', 0, '"+ obj.productName +"', '1', '"+ curDate +"', "+ obj.userID +", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                            if(obj.userID != 0 && obj.moduleId !=0)
                            {
                            saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                            }
                            using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                            {
                                rowAffected2 = con.Execute(cmd3);
                            }
        
                    }

                }   else{
                    cmd = "update public.\"invoiceDetail\" set \"qty\" = '" + obj.qty + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"invoiceNo\" = " + obj.invoiceNo + ";";
                    
                    if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected = con.Execute(cmd);
                    }  
                }             

                if(obj.invoiceNo == 0){
                    if(rowAffected > 0 && rowAffected2 > 0){
                        response = "Success";
                    }else{
                        response = "Server Issue";
                    }
                    
                }else{
                    if(rowAffected > 0){
                        response = "Success";
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