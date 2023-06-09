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
    public class PosDashboardController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3;
        private string subconStr;

        public PosDashboardController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }
        
        [HttpGet("getTodaySaleTransaction")]
        public IActionResult getTodaySaleTransaction(int branchid,int businessid,int companyid,int userID)
        {
            try
            {
                cmd = "select * from public.\"view_todaySaleTransaction\" where \"branchid\" = " + branchid + " AND \"businessid\" = " + businessid + " AND \"companyid\" = " + companyid + "";
                subconStr = userCredentials.FindMe(userID);
                var appMenu = dapperQuery.StrConQry<SaleTransactionDashboard>(cmd, subconStr);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getTodaySaleAmount")]
        public IActionResult getTodaySaleAmount(int branchid,int businessid,int companyid,int userID)
        {
            try
            {
                cmd = "select * from public.\"view_todaySaleAmount\" where \"branchid\" = " + branchid + " AND \"businessid\" = " + businessid + " AND \"companyid\" = " + companyid + "";
                subconStr = userCredentials.FindMe(userID);
                var appMenu = dapperQuery.StrConQry<SaleAmountDashboard>(cmd, subconStr);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getTopSales")]
        public IActionResult getTopSales(int branchid,int businessid,int companyid,int userID)
        {
            try
            {
                cmd = "select * from public.\"view_topSellingItem\" where \"branchid\" = " + branchid + " AND \"businessid\" = " + businessid + " AND \"companyid\" = " + companyid + "";
                subconStr = userCredentials.FindMe(userID);
                var appMenu = dapperQuery.StrConQry<TopSalesDashboard>(cmd, subconStr);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getCoaTypeSummary")]
        public IActionResult getCoaTypeSummary(string fromDate, string toDate,int branchid,int businessid,int companyid,int userID)
        {
            try
            {
                cmd = "select * from fn_dash_coa_type_summary('" + fromDate + "', '" + toDate + "') where \"branchid\" = " + branchid + " AND \"businessid\" = " + businessid + " AND \"companyid\" = " + companyid + "";
                subconStr = userCredentials.FindMe(userID);
                var appMenu = dapperQuery.StrConQry<COATypeSummaryDashboard>(cmd, subconStr);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getUnderStock")]
        public IActionResult getUnderStock(int branchid,int businessid,int companyid,int userID)
        {
            try
            {
                cmd = "select * from fn_dash_under_stock(Current_date) where \"branchid\" = " + branchid + " AND \"businessid\" = " + businessid + " AND \"companyid\" = " + companyid + "";
                subconStr = userCredentials.FindMe(userID);
                var appMenu = dapperQuery.StrConQry<UnderStockDashboard>(cmd, subconStr);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getMonthlyExpense")]
        public IActionResult getMonthlyExpense(string fromDate, string toDate,int userID)
        {
            try
            {
                cmd = "select * from public.fn_dash_monthly_expense_income('" + fromDate + "', '" + toDate + "')";
                subconStr = userCredentials.FindMe(userID);
                var appMenu = dapperQuery.StrConQry<MonthlyExpenseDashboard>(cmd, subconStr);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getDailySales")]
        public IActionResult getDailySales(string fromDate, string toDate,int branchid,int businessid,int companyid,int userID)
        {
            try
            {
                cmd = "select * from public.fn_dash_daily_sale('" + fromDate + "', '" + toDate + "') where \"branchid\" = " + branchid + " AND \"businessid\" = " + businessid + " AND \"companyid\" = " + companyid + "";
                subconStr = userCredentials.FindMe(userID);
                var appMenu = dapperQuery.StrConQry<DailySalesDashboard>(cmd, subconStr);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getMonthlySales")]
        public IActionResult getMonthlySales(string fromDate, string toDate,int branchid,int businessid,int companyid,int userID)
        {
            try
            {
                cmd = "select * from public.fn_dash_monthly_sale('" + fromDate + "', '" + toDate + "') where \"branchid\" = " + branchid + " AND \"businessid\" = " + businessid + " AND \"companyid\" = " + companyid + "";
                subconStr = userCredentials.FindMe(userID);
                var appMenu = dapperQuery.StrConQry<MonthlySalesDashboard>(cmd, subconStr);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
    }
}