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
    public class BusinessController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3;

        public BusinessController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }
        
        [HttpGet("getBusiness")]
        public IActionResult getBusiness(int companyID)
        {
            try
            {
                if(companyID == 0){
                    cmd = "SELECT * FROM view_business";
                }else{
                    cmd = "SELECT * FROM view_business where \"companyID\" = " + companyID + "";
                }
                var appMenu = dapperQuery.Qry<Business>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("saveBusiness")]
        public IActionResult saveBusiness(BusinessCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                var found = false;
                var businessName = "";
                var newBusinessID = 0; 
                // var newUserRoleID = 0;


                List<BusinessCreation> appMenuBusiness = new List<BusinessCreation>();
                cmd = "select \"businessFullName\" from tbl_business where \"businessFullName\" ='" + obj.businessFullName + "'";
                appMenuBusiness = (List<BusinessCreation>)dapperQuery.Qry<BusinessCreation>(cmd, _dbCon);

                List<BusinessCreation> appMenuBusinessID = new List<BusinessCreation>();
                cmd3 = "select \"businessID\" from tbl_business order by \"businessID\" desc limit 1";
                appMenuBusinessID = (List<BusinessCreation>)dapperQuery.Qry<BusinessCreation>(cmd3, _dbCon);
                if(appMenuBusinessID.Count == 0)
                    {
                        newBusinessID = 1;    
                    }else{
                        newBusinessID = appMenuBusinessID[0].businessID + 1;
                    }

                if (appMenuBusiness.Count > 0)
                        businessName = appMenuBusiness[0].businessFullName;

                if(businessName == "")
                {
                    if(obj.businessEDoc == null || obj.businessEDoc == "")
                    {
                    cmd2 = "INSERT INTO public.tbl_business(\"businessID\", \"businessFullName\", \"businessShortName\", \"businessAddress\", email, \"phoneNo\", \"mobileNo\", \"companyID\", \"createdOn\", \"createdBy\", \"isDeleted\") values (" + newBusinessID + ",'" + obj.businessFullName + "','" + obj.businessShortName + "','" + obj.businessAddress + "','" + obj.email + "', '" + obj.phoneNo + "', '" + obj.mobileNo + "', '" + obj.companyID + "', '" + curDate + "', '" + obj.userID + "', 0)";
                    }else{
                    cmd2 = "INSERT INTO public.tbl_business(\"businessID\", \"businessFullName\", \"businessShortName\", \"businessAddress\", email, \"phoneNo\", \"mobileNo\", \"companyID\", \"createdOn\", \"createdBy\", \"isDeleted\", \"businessEDoc\") values (" + newBusinessID + ",'" + obj.businessFullName + "','" + obj.businessShortName + "','" + obj.businessAddress + "','" + obj.email + "', '" + obj.phoneNo + "', '" + obj.mobileNo + "', '" + obj.companyID + "', '" + curDate + "', '" + obj.userID + "', 0," + newBusinessID + '.' + obj.businessEDocExtenstion + ")";
                    }
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
                    
                    if (obj.businessEDoc != null && obj.businessEDoc != "")
                    {
                        dapperQuery.saveImageFile(
                            obj.businessEDoc,
                            newBusinessID.ToString(),
                            obj.businessEDocPath,
                            obj.businessEDocExtenstion);
                    }

                    response = "Success";
                    return Ok(new { message = response });
                }
                else
                {
                    if (found == true)
                    {
                        response = "Business name already exist";
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

        [HttpPost("updateBusiness")]
        public IActionResult updateBusiness(BusinessCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                
                if (obj.businessEDoc == null || obj.businessEDoc == "")
                {
                    cmd = "update public.tbl_business set \"businessFullName\" = '" + obj.businessFullName + "', \"businessAddress\" = '" + obj.businessAddress +"', \"businessShortName\" = '"+ obj.businessShortName +"', \"email\" = '"+ obj.email +"', \"mobileNo\" = '"+ obj.mobileNo +"', \"phoneNo\" = '"+ obj.phoneNo +"', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = '" + obj.userID + "' where \"businessID\"=" + obj.businessID + "";    
                }
                else
                {
                    cmd = "update public.tbl_business set \"businessFullName\" = '" + obj.businessFullName + "', \"businessAddress\" = '" + obj.businessAddress +"', \"businessShortName\" = '"+ obj.businessShortName +"', \"email\" = '"+ obj.email +"', \"mobileNo\" = '"+ obj.mobileNo +"', \"phoneNo\" = '"+ obj.phoneNo +"', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = '" + obj.userID + "', \"businessEDoc\" = '" + obj.businessID + '.' + obj.businessEDocExtenstion + "' where \"businessID\"=" + obj.businessID + "";
                }
                
                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                if (rowAffected > 0)
                {
                    
                    if (obj.businessEDoc != null && obj.businessEDoc != "")
                    {
                        dapperQuery.saveImageFile(
                            obj.businessEDoc,
                            obj.businessID.ToString(),
                            obj.businessEDocPath,
                            obj.businessEDocExtenstion);
                    }

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

        [HttpPost("deleteBusiness")]
        public IActionResult deleteBusiness(BusinessCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";


                cmd2 = "UPDATE public.tbl_business SET \"isDeleted\"=1 where \"businessID\"=" + obj.businessID + "";
            
                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd2);
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