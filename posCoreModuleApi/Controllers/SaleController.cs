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
    public class SaleController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2, cmd3, cmd4, cmd5, cmd6;
        public string saveConStr;

        public SaleController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getSaleReturn")]
        public IActionResult getSaleReturn(int branchId,int invoiceNo,int userID, int moduleId)
        {
            try
            {
                cmd = "SELECT id.\"productID\", sum(case when i.\"invoiceType\" = 'SR' then id.qty * (-1) else id.qty end )as qty FROM invoice i JOIN \"invoiceDetail\" id ON id.\"invoiceNo\" = i.\"invoiceNo\" where i.\"invoiceNo\" = " + invoiceNo + " or i.\"refInvoiceNo\" = " + invoiceNo + " and i.\"branchid\" = " + branchId + " and id.\"productID\" is not null GROUP By id.\"productID\"";

                // cmd = "select * from \"view_saleReturn\" where \"invoiceNo\" = " + invoiceNo + " and \"isDeleted\"::int = 0 and \"productID\" is not null";
                var appMenu = _dapperQuery.StrConQry<SaleReturn>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }


        [HttpGet("getOrder")]
        public IActionResult getOrder(int branchId, int userID, int moduleId)
        {
            try
            {
                cmd = "SELECT o.\"orderID\", o.\"customerID\", o.\"customerName\", o.\"orderType\", SUM(od.\"qty\" * od.\"price\") AS \"total\", o.remarks, o.\"refOrderID\" FROM \"Order\" o INNER JOIN \"OrderDetail\" od ON o.\"orderID\" = od.\"orderID\" where o.status ='1' and o.\"isDeleted\"::int = 0 and o.branchid = " + branchId + " GROUP BY o.\"orderID\", o.\"customerName\", o.\"customerID\",o.remarks ";

                // cmd = "select * from \"view_saleReturn\" where \"invoiceNo\" = " + invoiceNo + " and \"isDeleted\"::int = 0 and \"productID\" is not null";
                var appMenu = _dapperQuery.StrConQry<Order>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getOrderDetail")]
        public IActionResult getOrder(int branchId, int orderNo, int userID, int moduleId)
        {
            try
            {
                cmd = "select * from \"OrderDetail\" where \"orderID\"=" + orderNo + " and \"isDeleted\"::int = 0 and branchid = "+branchId+"";

                // cmd = "select * from \"view_saleReturn\" where \"invoiceNo\" = " + invoiceNo + " and \"isDeleted\"::int = 0 and \"productID\" is not null";
                var appMenu = _dapperQuery.StrConQry<OrderDetail>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveOrder")]
        public IActionResult saveOrder(InvoiceCreation obj)
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
                var coaID = 0;
                var response = "";
                List<Order> appMenuOrder = new List<Order>();

                var total = 0.0;
                var totalAmount = 0.0;

            if(obj.refOrderNo == 0){
            List<Order> appMenuOrderID = new List<Order>();
                cmd2 = "select (max(\"orderID\")+1) as \"orderID\" from \"Order\" ";

                appMenuOrderID = (List<Order>)_dapperQuery.StrConQry<Order>(cmd2, obj.userID,obj.moduleId);

                if (appMenuOrderID.Count > 0)
                {
                    obj.refOrderNo = appMenuOrderID[0].orderID;
                }

            }
    
                cmd = "insert into public.\"Order\" (\"orderDate\", \"customerID\", \"customerName\", status, \"orderType\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\",\"remarks\",\"refOrderID\") values ('" + obj.invoiceDate + "', '" + obj.partyID + "', '" + obj.customerName + "', 1, '" + obj.orderType + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ",'" + obj.description + "', " + obj.refOrderNo + ")";

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
                    cmd2 = "SELECT \"orderID\" FROM public.\"Order\" order by \"orderID\" desc limit 1";
                    appMenuOrder = (List<Order>)_dapperQuery.StrConQry<Order>(cmd2, obj.userID,obj.moduleId);

                    var orderID = appMenuOrder[0].orderID;

                    //convert string to json data to insert in invoice detail table
                    var invObject = JsonConvert.DeserializeObject<List<InvoiceDetailCreation>>(obj.json);


                    //saving json data one by one in invoice detail table
                    foreach (var item in invObject)
                    {
                        cmd3 = "insert into public.\"OrderDetail\" (\"orderID\", \"productID\", \"qty\", \"costPrice\", price, \"productName\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + orderID + "', '" + item.productID + "', '" + item.qty + "', '" + item.costPrice + "', '" + item.salePrice + "', '" + item.productName + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

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

                return Ok(new { message = response, orderNo = appMenuOrder[0].orderID });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveSales")]
        public IActionResult saveSales(InvoiceCreation obj)
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
                var coaID = 0;
                var response = "";
                List<Invoice> appMenuInvoice = new List<Invoice>();
                // List<Invoice> appMenuBarcode = new List<Invoice>();
                var total = 0.0;
                var totalAmount = 0.0;

                List<Bank> appMenuBank = new List<Bank>();
                cmd2 = "select \"coaid\" from bank where \"isDeleted\"::int = 0 and \"bankID\"='" + obj.bankID + "'";
                appMenuBank = (List<Bank>)_dapperQuery.StrConQry<Bank>(cmd2, obj.userID,obj.moduleId);
                // appMenuBank = (List<Bank>)dapperQuery.QryResult<Bank>(cmd2, _dbCon);

                if (appMenuBank.Count > 0)
                {
                    coaID = appMenuBank[0].coaID;
                }
                

                if (obj.partyID == 0 && obj.bankID == 0)
                {
                    //In case of partyID is null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + obj.invoiceDate + "', '" + time + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'S', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                }
                else if (obj.partyID > 0 && obj.bankID ==0)
                {
                    //In case of partyID is not null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"partyID\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + obj.invoiceDate + "', '" + time + "', '" + obj.partyID + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'S', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                }
                else if (obj.partyID > 0 && obj.bankID > 0)
                {
                    // total = (obj.cashReceived + obj.bankcashReceived);
                    //In case of partyID and bankID is not null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"partyID\",\"bankID\",\"bankref\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\",\"bankcashReceived\") values ('" + obj.invoiceDate + "', '" + time + "', '" + obj.partyID + "','" + obj.bankID + "','" + obj.bankref + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'S', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ",'" + obj.bankcashReceived + "')";
                }

                else if (obj.partyID == 0 && obj.bankID > 0)
                {
                    total = (obj.cashReceived + obj.bankcashReceived);
                    //In case of partyID and bankID is not null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\",\"bankID\",\"bankref\", \"bankcashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + obj.invoiceDate + "', '" + time + "', '" + obj.bankID + "','" + obj.bankref + "', " + obj.bankcashReceived + ", " + obj.discount + ", '" + obj.change + "', 'S', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
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

                        total += (item.qty * item.salePrice);
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
                    if (obj.partyID > 0 && obj.bankID ==0 && (total - obj.cashReceived) > 0)
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

                    //in case of loan (udhaar) payment where partyID and bankID  is not null
                    if (obj.partyID > 0 && obj.bankID > 0 && (total - (obj.cashReceived + obj.bankcashReceived)) > 0)
                    {

                            cmd5 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + obj.bankcashReceived + "', 0, "+coaID+", '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

                            if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                            using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                            {
                                rowAffected4 = con.Execute(cmd5);
                            }

                         cmd5 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + obj.cashReceived + "', 0, 2, '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

                         if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected4 = con.Execute(cmd5);
                        }
                    

                        
                        totalAmount = total - (obj.cashReceived + obj.bankcashReceived);

                        cmd5 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + totalAmount + "', 0, 6, '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

                         if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected4 = con.Execute(cmd5);
                        }
                    }
                    //in case of  payment where partyID and bankID  is not null
                    if (obj.partyID > 0 && obj.bankID > 0 && (total - (obj.cashReceived + obj.bankcashReceived)) <= 0)               
                      {

                            cmd5 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + obj.bankcashReceived + "', 0, "+coaID+", '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

                            if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                            using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                            {
                                rowAffected4 = con.Execute(cmd5);
                            }

                         cmd5 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + obj.cashReceived + "', 0, 2, '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

                         if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected4 = con.Execute(cmd5);
                        }
                    
                    }

                    
                    

                    //in case of cash payment and bankcashReceived = 0
                    if (obj.cashReceived > 0 && obj.bankcashReceived ==0)
                    {
                        

                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + obj.cashReceived + "', 0, '2', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

                         if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected3 = con.Execute(cmd4);
                        }
                    }
                    //in case of  bankcashReceived
                    if (obj.cashReceived ==0  && obj.bankcashReceived > 0)
                    {
                        

                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', '" + obj.bankcashReceived + "', 0, "+coaID+", '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

                         if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected3 = con.Execute(cmd4);
                        }
                    }


                    if(obj.businessTypeID == 3){
                        
                        //convert string to json data to insert in invoice detail table
                        var orderObject = JsonConvert.DeserializeObject<List<Order>>(obj.orderJson);


                        //saving json data one by one in invoice detail table
                        foreach (var item in orderObject)
                        {
                        
                            cmd = "update \"Order\" set status = 2 where \"orderID\" = "+ item.orderID +";";
                
                            if(obj.userID != 0 && obj.moduleId !=0)
                            {
                            saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                            }
                            using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                            {
                                rowAffected3 = con.Execute(cmd);
                            }

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

        [HttpPost("saveSaleReturn")]
        public IActionResult saveSaleReturn(InvoiceCreation obj)
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

                //getting invoice date from current saved invoice
                cmd2 = "SELECT \"invoiceDate\" FROM public.invoice where \"invoiceNo\" = " + obj.refInvoiceNo + " order by \"invoiceNo\" desc limit 1";
                appMenuInvoice = (List<Invoice>)_dapperQuery.StrConQry<Invoice>(cmd2, obj.userID,obj.moduleId);

                var invoiceDate = appMenuInvoice[0].invoiceDate;


                if (obj.partyID == 0)
                {
                    //In case of partyID is null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"refInvoiceNo\", \"refInvoiceDate\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + obj.refInvoiceDate + "', '" + time + "','" + obj.refInvoiceNo + "','" + invoiceDate + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'SR', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                }
                else
                {
                    //In case of partyID is not null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"partyID\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + obj.refInvoiceDate + "', '" + time + "', '" + obj.partyID + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'SR', '" + obj.description + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
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
                    if (obj.cashReceived == 0)
                    {

                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values ('" + invoiceNo + "', 0, '" + -1 * obj.change + "', '2', '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";

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

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("deleteSales")]
        public IActionResult deleteSales(InvoiceCreation obj)
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

        [HttpPost("deleteOrder")]
        public IActionResult deleteOrder(Order obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";

                cmd = "update \"Order\" set \"isDeleted\" = B'1' where \"orderID\" = "+ obj.orderID +";";
                
                cmd2 = "update \"OrderDetail\" set \"isDeleted\" = B'1' where \"orderID\" = "+ obj.orderID +";";

                if(obj.userID != 0 && obj.moduleId !=0)
                {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected = con.Execute(cmd2);
                    }
                    
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected2 = con.Execute(cmd);
                    }
                }

                if (rowAffected > 0 && rowAffected2 > 0)
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