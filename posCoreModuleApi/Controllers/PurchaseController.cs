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
    public class PurchaseController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2, cmd3, cmd4, cmd5, cmd6;
        public string saveConStr;

        public PurchaseController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getPurchaseReturn")]
        public IActionResult getPurchaseReturn(int branchID, int invoiceNo,int userID, int moduleId)
        {
            try
            {
                cmd = "SELECT id.\"productID\", sum(case when i.\"invoiceType\" = 'PR' then id.qty * (-1) else id.qty end )as qty FROM invoice i JOIN \"invoiceDetail\" id ON id.\"invoiceNo\" = i.\"invoiceNo\" where i.\"invoiceNo\" = " + invoiceNo + " or i.\"refInvoiceNo\" = " + invoiceNo + "  and i.\"branchid\" = " + branchID + " and id.\"productID\" is not null GROUP By id.\"productID\"";

                // cmd = "select * from \"view_saleReturn\" where \"invoiceNo\" = " + invoiceNo + " and \"isDeleted\"::int = 0 and \"productID\" is not null";

                var appMenu = _dapperQuery.StrConQry<SaleReturn>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("savePurchase")]
        public IActionResult savePurchase(InvoiceCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                int rowAffected4 = 0;
                int rowAffected5 = 0;
                var response = "";
                List<Invoice> appMenuInvoice = new List<Invoice>();
                // List<Invoice> appMenuBarcode = new List<Invoice>();
                var total = 0.0;


                if (obj.partyID == 0)
                {
                    //In case of partyID is null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + obj.invoiceDate + "', '" + time + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'P', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                }
                else
                {
                    //In case of partyID is not null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"partyID\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + obj.invoiceDate + "', '" + time + "', '" + obj.partyID + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'P', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                }

                if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }

                using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                {
                    rowAffected = con.Execute(cmd);
                }

                //confirmation of data saved in invoice
                if (rowAffected > 0)
                {

                    //getting last saved invoice no
                    cmd2 = "SELECT \"invoiceNo\" FROM public.invoice order by \"invoiceNo\" desc limit 1";
                    appMenuInvoice = (List<Invoice>)_dapperQuery.StrConQry<Invoice>(cmd2, obj.userID,obj.moduleId);

                    var invoiceNo = appMenuInvoice[0].invoiceNo;

                    //convert string to json data to insert in invoice detail table
                    var invObject = JsonConvert.DeserializeObject<List<InvoiceDetailCreation>>(obj.json);


                    //saving json data one by one in invoice detail table
                    foreach (var item in invObject)
                    {
                        cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"qty\", \"costPrice\", \"salePrice\", \"debit\", \"credit\", \"discount\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + item.productID + "', '" + item.qty + "', '" + item.costPrice + "', '" + item.salePrice + "', '" + item.qty * item.salePrice + "', 0, '" + item.discount + "', '" + item.productName + "', '1', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                        if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected2 = con.Execute(cmd3);
                        }

                        total += item.salePrice;
                    }

                    total -= obj.discount;

                    //in case of giving discount to over all bill
                    if (obj.discount > 0)
                    {

                        cmd6 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', 0, '" + obj.discount + "', '3', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                        if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected5 = con.Execute(cmd6);
                        }

                    }

                    //in case of loan (udhaar) payment where partyID is not null
                    if (obj.partyID > 0 && (total - obj.cashReceived) > 0)
                    {
                        total -= obj.cashReceived;

                        cmd5 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', 0, '" + total + "', '6', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                        if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected4 = con.Execute(cmd5);
                        }
                    }

                    //in case of cash payment
                    if (obj.cashReceived > 0)
                    {

                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', 0, '" + obj.cashReceived + "', '2', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                        if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected3 = con.Execute(cmd4);
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

                return Ok(new { message = response, invoiceNo = appMenuInvoice[0].invoiceNo });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("savePurchaseReturn")]
        public IActionResult savePurchaseReturn(InvoiceCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                int rowAffected4 = 0;
                int rowAffected5 = 0;
                var response = "";
                List<Invoice> appMenuInvoice = new List<Invoice>();
                // List<Invoice> appMenuBarcode = new List<Invoice>();
                var total = 0.0;
                // var invoiceDate;

                //getting invoice date from current saved invoice
                cmd2 = "SELECT \"invoiceDate\" FROM public.invoice where \"invoiceNo\" = " + obj.refInvoiceNo + " order by \"invoiceNo\" desc limit 1";
                appMenuInvoice = (List<Invoice>)_dapperQuery.StrConQry<Invoice>(cmd2, obj.userID,obj.moduleId);
                // if (appMenuInvoice.Count > 0)
                var invoiceDate = appMenuInvoice[0].invoiceDate;    
                


                if (obj.partyID == 0)
                {
                    //In case of partyID is null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"refInvoiceNo\", \"refInvoiceDate\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceDate + "', '" + time + "','" + obj.refInvoiceNo + "','" + obj.refInvoiceDate + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'PR', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                }
                else
                {
                    //In case of partyID is not null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"partyID\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceDate + "', '" + time + "', '" + obj.partyID + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'PR', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                }
                if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }

                using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                {
                    rowAffected = con.Execute(cmd);
                }

                //confirmation of data saved in invoice
                if (rowAffected > 0)
                {

                    //getting last saved invoice no
                    cmd2 = "SELECT \"invoiceNo\" FROM public.invoice order by \"invoiceNo\" desc limit 1";
                    appMenuInvoice = (List<Invoice>)_dapperQuery.StrConQry<Invoice>(cmd2, obj.userID,obj.moduleId);

                    var invoiceNo = appMenuInvoice[0].invoiceNo;

                    //convert string to json data to insert in invoice detail table
                    var invObject = JsonConvert.DeserializeObject<List<InvoiceDetailCreation>>(obj.json);


                    //saving json data one by one in invoice detail table
                    foreach (var item in invObject)
                    {
                        cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"qty\", \"costPrice\", \"salePrice\", \"debit\", \"credit\", \"discount\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + item.productID + "', '" + item.qty + "', '" + item.costPrice + "', '" + item.salePrice + "', 0, '" + item.qty * item.salePrice + "', '" + item.discount + "', '" + item.productName + "', '1', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                    
                        if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected2 = con.Execute(cmd3);
                        }

                        total += item.salePrice;
                    }

                    total -= obj.discount;

                    //in case of giving discount to over all bill
                    if (obj.discount > 0)
                    {

                        cmd6 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + obj.discount + "', 0, '3', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

                            if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected5 = con.Execute(cmd6);
                        }

                    }

                    //in case of loan (udhaar) payment where partyID is not null
                    if (obj.partyID > 0 && (total - obj.cashReceived) > 0)
                    {
                        total -= obj.cashReceived;

                        cmd5 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + total + "', 0, '6', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

                        if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected4 = con.Execute(cmd5);
                        }
                    }

                    //in case of cash payment
                    if (obj.cashReceived == 0)
                    {

                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + -1 * obj.change + "', 0, '2', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

                        if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected3 = con.Execute(cmd4);
                        }
                    }

                }

                if (rowAffected > 0 && rowAffected2 > 0 )
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

        [HttpPost("deletePurchase")]
        public IActionResult deletePurchase(InvoiceCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                var response = "";

                // cmd = "update product set \"isDeleted\" = B'1', \"modifiedOn\" = '"+ curDate +"', \"modifiedBy\" = "+ obj.userID +" where \"productID\" = "+ obj.productID +";";

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
                    response = "Invalid Input Error";
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