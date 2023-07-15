using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMISModuleApi.Services;
using Microsoft.Extensions.Options;
using CMISModuleApi.Configuration;
using CMISModuleApi.Entities;
using Dapper;
using System.Data;
using Npgsql;
using System.Collections.Generic;

namespace CMISModuleApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3;

        public BranchController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getBranch")]
        public IActionResult getBranch(int businessID)
        {
            try
            {
                if(businessID == 0){
                    cmd = "SELECT * FROM view_branch";
                }else{
                    cmd = "SELECT * FROM view_branch where \"businessID\" = " + businessID + "";
                }
                var appMenu = dapperQuery.Qry<Branch>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getBusniessName")]
        public IActionResult getBusniessName(int branchID)
        {
            try
            {
                if(branchID == 0){
                    cmd = "SELECT * FROM view_businessname";
                }else{
                    cmd = "SELECT * FROM view_businessname where \"branchID\" = " + branchID + "";
                }
                var appMenu = dapperQuery.Qry<BusinessName>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }


        [HttpPost("saveBranch")]
        public IActionResult saveBranch(BranchCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                var found = false;
                var branchName = "";
                var newBranchID = 0; 
                // var newUserRoleID = 0;


                List<BranchCreation> appMenuBranch = new List<BranchCreation>();
                cmd = "select b.\"branchName\" from tbl_branch b inner join tbl_business_branch bb on bb.\"branchID\" = b.\"branchID\" where b.\"branchName\" = '" + obj.branchName + "' AND bb.\"businessID\" = " + obj.businessID + " ";
                appMenuBranch = (List<BranchCreation>)dapperQuery.Qry<BranchCreation>(cmd, _dbCon);

                List<BranchCreation> appMenuBranchID = new List<BranchCreation>();
                cmd3 = "select \"branchID\" from tbl_branch order by \"branchID\" desc limit 1";
                appMenuBranchID = (List<BranchCreation>)dapperQuery.Qry<BranchCreation>(cmd3, _dbCon);
                if(appMenuBranchID.Count == 0)
                {
                    newBranchID = 1;    
                }else{
                    newBranchID = appMenuBranchID[0].branchID + 1;
                }

                if (appMenuBranch.Count > 0)
                        branchName = appMenuBranch[0].branchName;

                if(branchName == "")
                {
                    cmd2 = "INSERT INTO public.tbl_branch(\"branchID\", \"branchName\", \"branchAddress\", email, \"phoneNo\", \"mobileNo\", \"createdOn\", \"createdBy\", \"businessID\", \"isDeleted\") values (" + newBranchID + ",'" + obj.branchName + "', '" + obj.branchAddress + "','" + obj.email + "', '" + obj.phoneNo + "', '" + obj.mobileNo + "', '" + curDate + "', '" + obj.userID + "', '" + obj.businessID + "', 0)";
                }
                else
                {
                    found=true;
                }
                if (found == false)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected = con.Execute(cmd2);
                    }
                    
                }

                if (rowAffected > 0 )
                {   

                    var newBusBranchID = 0;
                    List<BranchCreation> appMenuBranchBusinessID = new List<BranchCreation>();
                    
                    cmd = "select \"businessBranchID\" from tbl_business_branch order by \"businessBranchID\" desc limit 1";
                    appMenuBranchBusinessID = (List<BranchCreation>)dapperQuery.Qry<BranchCreation>(cmd, _dbCon);

                    if(appMenuBranchBusinessID.Count == 0)
                    {
                        newBusBranchID = 1;    
                    }else{
                        newBusBranchID = appMenuBranchBusinessID[0].businessBranchID + 1;
                    }

                    cmd2 = "INSERT INTO public.tbl_business_branch(\"businessBranchID\", \"businessID\", \"branchID\", \"isDeleted\") values (" + newBusBranchID + ",'" + obj.businessID + "', '" + newBranchID + "', 0)";
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected2 = con.Execute(cmd2);
                    }

                    if(rowAffected2 > 0){
                        response = "Success";
                        return Ok(new { message = response });
                    }else{
                        response = "Server Issue";
                        return BadRequest(new { message = response });
                    }
                    
                }
                else
                {
                    if (found == true)
                    {
                        response = "Branch name already exist";
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                return BadRequest(new { message = response });

                }

            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("updateBranch")]
        public IActionResult updateBranch(BranchCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                
                cmd = "update public.tbl_branch set \"branchName\" = '" + obj.branchName + "', \"branchAddress\" = '" + obj.branchAddress +"', \"email\" = '"+ obj.email +"', \"mobileNo\" = '"+ obj.mobileNo +"', \"phoneNo\" = '"+ obj.phoneNo +"', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = '" + obj.userID + "' where \"branchID\"=" + obj.branchID + "";    
                
                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                if (rowAffected > 0)
                {
                    response = "Success";
                    return Ok(new { message = response });
                }
                else
                {
                    response = "something went wrong";
                    
                    return BadRequest(new { message = response });
                }

            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("deleteBranch")]
        public IActionResult deleteBranch(BranchCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";


                cmd2 = "UPDATE public.tbl_branch SET \"isDeleted\"=1 where \"branchID\"=" + obj.branchID + "";
                cmd3 = "UPDATE public.tbl_business_branch SET \"isDeleted\"=1 where \"businessBranchID\"=" + obj.businessBranchID + "";
            
                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd2);
                }

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd3);
                }

                if (rowAffected > 0)
                {
                    response = "Success";
                return Ok(new { message = response });

                }
                else
                {
                    
                    response = "Try again";
                    
                return BadRequest(new { message = response });

                }

            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }


    }
}