using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using bachatOnlineModuleApi.Services;
using Microsoft.Extensions.Options;
using bachatOnlineModuleApi.Configuration;
using bachatOnlineModuleApi.Entities;
using System.Data;
using Npgsql;
using System.Collections.Generic;
using MySqlConnector;
using Dapper;
using Newtonsoft.Json;

namespace bachatOnlineApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3, cmd4, cmd5;

        public ProductController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getTestData")]
        public IActionResult getTestData()
        {
            return Ok("OK");
        }

        [HttpGet("getOnlineProduct")]
        public IActionResult getOnlineProduct(int branchid,int businessid,int companyid)
        {
            try
            {
                cmd = "select * from public.\"view_storeProducts\" where branchid = " + branchid + " and businessid = " + businessid + " and companyid = " + companyid + " ";
                var appMenu = dapperQuery.QryResult<Product>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getCheckQty")]
        public IActionResult getCheckQty(int invoiceDetailID, int qty)
        {
            try
            {
                cmd = "SELECT \"invoiceDetailID\", qty,branchid,businessid,companyid FROM \"invoiceDetail\" where \"invoiceDetailID\" = '" + invoiceDetailID + "' AND \"isDeleted\"::int = 0 ";
                var appMenu = (List<Product>)dapperQuery.QryResult<Product>(cmd, _dbCon);

                var availQty = appMenu[0].qty - qty;

                if (availQty >= 0)
                {
                    return Ok(new { msg = true });
                }
                else
                {
                    return Ok(new { msg = false, qty = appMenu[0].qty });
                }
                // return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getPlaceOrder")]
        public IActionResult getPlaceOrder(int businessid,int companyid)
        {
            try
            {
                cmd = "select * from public.\"Order\" where status = 'pend' OR status is null and businessid = " + businessid + " and companyid = " + companyid + "";
                var appMenu = dapperQuery.QryResult<Order>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getOrderDetail")]
        public IActionResult getOrderDetail(int orderID)
        {
            try
            {
                cmd = "select * from public.\"OrderDetail\" where \"orderID\" = " + orderID + "";
                var appMenu = dapperQuery.QryResult<OrderDetail>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("checkout")]
        public IActionResult checkout(OrderCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                var response = "";
                List<Order> appMenuOrder = new List<Order>();

                cmd = "insert into public.\"Order\" (\"orderDate\", \"customerName\", \"email\", \"mobile\", \"address\", \"createdOn\", \"isDeleted\",\"businessid\",\"companyid\") values ('" + curDate + "', '" + obj.customerName + "', '" + obj.email + "', '" + obj.mobile + "', '" + obj.address + "', '" + curDate + "', B'0'," + obj.businessid + ", " + obj.companyid + ")";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                //confirmation of data saved in order table
                if (rowAffected > 0)
                {

                    // //getting last saved invoice no
                    cmd2 = "SELECT \"orderID\" FROM public.\"Order\" order by \"orderID\" desc limit 1";
                    appMenuOrder = (List<Order>)dapperQuery.QryResult<Order>(cmd2, _dbCon);

                    var orderID = appMenuOrder[0].orderID;

                    //convert string to json data to insert in invoice detail table
                    var invObject = JsonConvert.DeserializeObject<List<OrderDetailCreation>>(obj.json);


                    //saving json data one by one in invoice detail table
                    foreach (var item in invObject)
                    {
                        // cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"locationID\", \"qty\", \"costPrice\", \"salePrice\", \"debit\", \"credit\", \"discount\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + item.productID + "', '" + item.locationID + "', '" + item.qty + "', '" + item.costPrice + "', '" + item.salePrice + "', 0, '" + item.qty * item.salePrice + "', '" + item.discount + "', '" + item.productName + "', '1', '" + curDate + "', " + obj.userID + ", B'0')";
                        cmd3 = "INSERT INTO public.\"OrderDetail\"(\"orderID\", \"productID\", \"productName\", qty, price, \"createdOn\", \"isDeleted\",\"businessid\",\"companyid\") VALUES ('" + orderID + "', '" + item.productID + "',  '" + item.productName + "',  '" + item.qty + "',  '" + item.salePrice + "', '" + curDate + "', B'0'," + obj.businessid + "," + obj.companyid + ")";
                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd3);
                        }

                        cmd4 = "SELECT \"invoiceDetailID\", qty FROM \"invoiceDetail\" where \"invoiceDetailID\" = '" + item.invoiceDetailID + "' AND \"isDeleted\"::int = 0 AND branchid = " + obj.branchid + " and businessid = " + obj.businessid + " and companyid = " + obj.companyid + "";
                        var appMenu = (List<Product>)dapperQuery.QryResult<Product>(cmd4, _dbCon);

                        var availQty = appMenu[0].qty - item.qty;

                        cmd5 = "update public.\"invoiceDetail\" set qty = " + availQty + " where \"invoiceDetailID\" = " + item.invoiceDetailID + ";";


                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected3 = con.Execute(cmd5);
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

        [HttpPost("placeOrder")]
        public IActionResult placeOrder(OrderCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.\"Order\" set status = '" + obj.status + "' where \"orderID\" = " + obj.orderID + ";";

                // cmd = "insert into public.\"Order\" (\"orderDate\", \"customerName\", \"email\", \"mobile\", \"address\", \"createdOn\", \"isDeleted\") values ('" + obj.orderDate + "', '" + obj.customerName + "', " + obj.email + ", " + obj.mobile + ", '" + obj.address + "', '" + curDate + "', B'0')";

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