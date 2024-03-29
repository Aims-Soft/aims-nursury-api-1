using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using reportApi.Services;
using Microsoft.Extensions.Options;
using reportApi.Configuration;
using reportApi.Entities;
using Dapper;
using System.Data;
using Npgsql;
using System.Collections.Generic;

namespace reportApi.Controllers
{
 
    [ApiController]
    [Route("[controller]")]
    public class FMISReportController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2;
        public string saveConStr;

        public FMISReportController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }
        
        [HttpGet("getLedgerReport")]
        public IActionResult getLedgerReport(int branchID,int coaID, string fromDate, string toDate,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.ledgerreport  where coaid = '" + coaID + "' and \"ledgerreport\".\"branchid\" = " + branchID + " and invoicedate BETWEEN '" + fromDate + "' AND '" + toDate + "'";

                var appMenu = _dapperQuery.StrConQry<Ledger>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getDailyProductSale")]
        public IActionResult getDailyProductSale(int branchID,int companyID,int businessID,string curDate,int userID, int moduleId)
        {
            try
            {
                cmd = "Select * From public.\"view_dailyProductSales\" Where \"branchID\" = " + branchID + " and companyid = " + companyID + " and businessid = " + businessID + " and \"invoiceDate\" = '" + curDate + "' Order By \"productName\" ASC";
                var appMenu = _dapperQuery.StrConQry<DailyProductSale>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getInvoiceDetail")]
        public IActionResult getInvoiceDetail(int companyid,int branchid,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.\"view_invoicedetail\"  where \"companyid\" = " + companyid + " and \"branchid\" = " + branchid + "";

                var appMenu = _dapperQuery.StrConQry<InvoiceDetail>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getPurchases")]
        public IActionResult getPurchases(int companyid,int branchid,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.\"view_purchase\"  where \"companyid\" = " + companyid + " and \"branchid\" = " + branchid + "";

                var appMenu = _dapperQuery.StrConQry<InvoiceDetail>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getInvoicePrintDetail")]
        public IActionResult getInvoicePrintDetail(int companyid,int branchid,string invoiceNo,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.\"view_invoiceprintdetail\"  where \"companyid\" = " + companyid + " and \"branchid\" = " + branchid + " and \"invoiceNo\" = '"+invoiceNo+"' and \"userID\" = "+userID+"";

                var appMenu = _dapperQuery.StrConQry<InvoicePrintDetail>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getPurchaseDetail")]
        public IActionResult getPurchaseDetail(int companyid,int branchid,string invoiceNo,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.\"view_purchaseDetail\"  where \"companyid\" = " + companyid + " and \"branchid\" = " + branchid + " and \"invoiceNo\" = '"+invoiceNo+"' and \"userID\" = "+userID+"";

                var appMenu = _dapperQuery.StrConQry<InvoicePrintDetail>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getPartyLedgerReport")]
        public IActionResult getPartyLedgerReport(int branchID,int partyID,int coaID, string fromDate, string toDate,int userID, int moduleId)
        {
            try
            {
                if (coaID ==0)
                {

                    cmd = "select * from public.partyledgerview  where partyledgerview.partyid = '" + partyID + "' and \"partyledgerview\".\"branchid\" = " + branchID + " and invoicedate BETWEEN '" + fromDate + "' AND '" + toDate + "'";
                }
                else
                {
                    cmd = "select * from public.partyledgerview  where partyledgerview.partyid = '" + partyID + "' and partyledgerview.coaid = '" + coaID + "' and \"partyledgerview\".\"branchid\" = " + branchID + " and invoicedate BETWEEN '" + fromDate + "' AND '" + toDate + "'";
                }

                var appMenu = _dapperQuery.StrConQry<PartyLedger>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getPresentStock")]
        public IActionResult getPresentStock(int branchID,int partyID, string fromDate, string toDate,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.partyledgerview  where partyledgerview.partyid = '" + partyID + "' and \"partyledgerview\".\"branchid\" = " + branchID + " and invoicedate BETWEEN '" + fromDate + "' AND '" + toDate + "'";

                var appMenu = _dapperQuery.StrConQry<PartyLedger>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getDailyCategorySale")]
        public IActionResult getDailyCategorySale(int branchID,int partyID, string fromDate, string toDate,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.partyledgerview  where partyledgerview.partyid = '" + partyID + "' and \"partyledgerview\".\"branchid\" = " + branchID + "  and invoicedate BETWEEN '" + fromDate + "' AND '" + toDate + "'";

                var appMenu = _dapperQuery.StrConQry<PartyLedger>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        [HttpGet("getDailySales")]
        public IActionResult getDailySales(int branchID,string startDate,string endDate,int userID, int moduleId)
        {
            try
            {
                cmd = "SELECT * FROM public.\"view_dailySales\" WHERE  \"view_dailySales\".\"branchid\" = " + branchID + " and \"view_dailySales\".\"invoiceDate\" BETWEEN '" + startDate + "' AND '" + endDate + "'";

                var appMenu = _dapperQuery.StrConQry<DailySales>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getItemWiseDailySales")]
        public IActionResult getItemWiseDailySales(int branchID,string startDate,string endDate,int userID, int moduleId)
        {
            try
            {
                cmd = "select p.\"productID\", p.\"productName\", SUM(ivd.qty) as qty, pp.\"costPrice\", pp.\"salePrice\" from product p join \"productPrice\" pp ON pp.\"productID\" = p.\"productID\" join \"invoiceDetail\" ivd ON ivd.\"productID\" = p.\"productID\" join invoice i ON i.\"invoiceNo\" = ivd.\"invoiceNo\" where i.\"invoiceDate\" >= '"+startDate+"' AND i.\"invoiceDate\" <= '"+endDate+"' and i.branchid = "+branchID+" group by p.\"productID\", p.\"productName\", pp.\"costPrice\", pp.\"salePrice\", i.\"invoiceType\" having i.\"invoiceType\"='S' ";

                var appMenu = _dapperQuery.StrConQry<DailySales>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getDailySalesByOrder")]
        public IActionResult getDailySalesByOrder(int branchID,string startDate,string endDate,int userID, int moduleId)
        {
            try
            {
                cmd = "SELECT * FROM public.\"view_dailySalesbyOrder\" WHERE \"view_dailySalesbyOrder\".\"branchid\" = " + branchID + " and \"view_dailySalesbyOrder\".\"invoiceDate\" BETWEEN '" + startDate + "' AND '" + endDate + "'";

                var appMenu = _dapperQuery.StrConQry<DailySalesByOrder>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        [HttpGet("getStockInStockOut")]
        public IActionResult getStockInStockOut(string invDate,int userID, int moduleId, int companyID, int businessID)
        {
            try
            {
                // cmd = "select * from public.\"productStockInStockOut\"('"+invDate+"', "+companyID+", "+businessID+")";
                cmd = "select  \"productID\", \"productName\", stockin, stockout, stockin-stockout as remainingstock, \"costPrice\" , \"costPrice\" *(stockin-stockout) as remainingcostprice  from public.\"productStockInStockOut\"('"+invDate+"', "+companyID+", "+businessID+")";

                var appMenu = _dapperQuery.StrConQry<StockInStockOut>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getPeriodicSale")]
        public IActionResult getPeriodicSale(int branchID,int partyID, string fromDate, string toDate,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.partyledgerview  where \"partyledgerview\".\"branchid\" = " + branchID + " and partyledgerview.partyid = '" + partyID + "' and invoicedate BETWEEN '" + fromDate + "' AND '" + toDate + "'";

                var appMenu = _dapperQuery.StrConQry<PartyLedger>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getPeriodicCategorySale")]
        public IActionResult getPeriodicCategorySale(int branchID,int partyID, string fromDate, string toDate,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.partyledgerview  where \"partyledgerview\".\"branchid\" = " + branchID + " and partyledgerview.partyid = '" + partyID + "' and invoicedate BETWEEN '" + fromDate + "' AND '" + toDate + "'";

                var appMenu = _dapperQuery.StrConQry<PartyLedger>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getCashReport")]
        public IActionResult getCashReport(int branchID, string invoiceDate, int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.view_dailycashreportdata where \"invoiceDate\" = '" + invoiceDate + "' and (\"invoiceType\" ='S' or \"invoiceType\" = 'SR') and \"coaID\"= 2 and \"branchid\" = " + branchID + "";

                var appMenu = _dapperQuery.StrConQry<CashReport>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getCreditReport")]
        public IActionResult getCreditReport(int branchID, string invoiceDate, int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.view_dailyreportdata where \"invoiceDate\" = '" + invoiceDate + "' and (\"invoiceType\" ='S' or \"invoiceType\" = 'SR') and \"coaID\"= 6 and \"branchid\" = " + branchID + "";

                var appMenu = _dapperQuery.StrConQry<CashReport>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
    }
}