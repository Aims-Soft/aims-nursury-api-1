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
    public class CompanyController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3;

        public CompanyController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }
        
        [HttpGet("getCompany")]
        public IActionResult getCompany()
        {
            try
            {
                cmd = "SELECT * FROM public.tbl_company where \"isDeleted\"::int = 0";
                var appMenu = dapperQuery.Qry<Company>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("saveCompany")]
        public IActionResult saveCompany(CompanyCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                var found = false;
                var companyName = "";
                var newCompanyID = 0; 
                // var newUserRoleID = 0;


                List<CompanyCreation> appMenuCompany = new List<CompanyCreation>();
                cmd = "select \"companyFullName\" from public.tbl_company where \"companyFullName\" ='" + obj.companyFullName + "'";
                appMenuCompany = (List<CompanyCreation>)dapperQuery.Qry<CompanyCreation>(cmd, _dbCon);

                List<CompanyCreation> appMenuCompanyID = new List<CompanyCreation>();
                cmd3 = "select \"companyID\" from public.tbl_company order by \"companyID\" desc limit 1";
                appMenuCompanyID = (List<CompanyCreation>)dapperQuery.Qry<CompanyCreation>(cmd3, _dbCon);
                if(appMenuCompanyID.Count == 0)
                    {
                        newCompanyID = 1;    
                    }else{
                        newCompanyID = appMenuCompanyID[0].companyID+1;
                    }

                if (appMenuCompany.Count > 0)
                        companyName = appMenuCompany[0].companyFullName;

                if(companyName=="")
                {
                    if(obj.companyEDoc == null || obj.companyEDoc == "")
                    {
                    cmd2 = "insert into public.tbl_company (\"companyID\", \"companyFullName\", \"companyAddress\", \"companyNtn\", \"companyStrn\", \"companyRegistrationNo\", \"companyShortName\", email, \"mobileNo\", \"phoneNo\", \"createdOn\", \"createdBy\", \"isDeleted\") values (" + newCompanyID + ",'" + obj.companyFullName + "','" + obj.companyAddress + "','" + obj.companyNtn + "'," + obj.companyStrn + ", '" + obj.companyRegistrationNo + "', '" + obj.companyShortName + "', '" + obj.email + "', '" + obj.mobileNo + "', '" + obj.phoneNo + "', '" + curDate + "', '" + obj.userID + "', 0)";
                    }else{
                    cmd2 = "insert into public.tbl_company (\"companyID\", \"companyFullName\", \"companyAddress\", \"companyNtn\", \"companyStrn\", \"companyRegistrationNo\", \"companyShortName\", email, \"mobileNo\", \"phoneNo\", \"createdOn\", \"createdBy\", \"isDeleted\", \"companyEDoc\") values (" + newCompanyID + ",'" + obj.companyFullName + "','" + obj.companyAddress + "','" + obj.companyNtn + "'," + obj.companyStrn + ", '" + obj.companyRegistrationNo + "', '" + obj.companyShortName + "', '" + obj.email + "', '" + obj.mobileNo + "', '" + obj.phoneNo + "', '" + curDate + "', '" + obj.userID + "', 0," + newCompanyID + '.' + obj.companyEDocExtenstion + ")";
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
                    
                    if (obj.companyEDoc != null && obj.companyEDoc != "")
                    {
                        dapperQuery.saveImageFile(
                            obj.companyEDoc,
                            newCompanyID.ToString(),
                            obj.companyEDocPath,
                            obj.companyEDocExtenstion);
                    }

                    response = "Success";
                    return Ok(new { message = response });
                }
                else
                {
                    if (found == true)
                    {
                        response = "Company name already exist";
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

        [HttpPost("updateCompany")]
        public IActionResult updateCompany(CompanyCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                
                if (obj.companyEDocPath == null && obj.companyEDocPath == "")
                {
                    cmd = "update public.tbl_company set \"companyFullName\" = '" + obj.companyFullName + "', \"companyAddress\" = '" + obj.companyAddress +"', \"companyNtn\" = '"+ obj.companyNtn +"', \"companyStrn\" = '"+ obj.companyStrn +"', \"companyRegistrationNo\" = '"+ obj.companyRegistrationNo +"', \"email\" = '"+ obj.email +"', \"mobileNo\" = '"+ obj.mobileNo +"', \"phoneNo\" = '"+ obj.phoneNo +"', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = '" + obj.userID + "' where \"companyID\"="+obj.companyID+"";    
                }
                else
                {
                    cmd = "update public.tbl_company set \"companyFullName\" = '" + obj.companyFullName + "', \"companyAddress\" = '" + obj.companyAddress +"', \"companyNtn\" = '"+ obj.companyNtn +"', \"companyStrn\" = '"+ obj.companyStrn +"', \"companyRegistrationNo\" = '"+ obj.companyRegistrationNo +"', \"email\" = '"+ obj.email +"', \"mobileNo\" = '"+ obj.mobileNo +"', \"phoneNo\" = '"+ obj.phoneNo +"', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = '" + obj.userID + "', \"companyEDoc\" = '" + obj.companyID + '.' + obj.companyEDocExtenstion + "' where \"companyID\"="+obj.companyID+"";
                }
                
                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                if (rowAffected > 0)
                {
                    
                    if (obj.companyEDoc != null && obj.companyEDoc != "")
                    {
                        dapperQuery.saveImageFile(
                            obj.companyEDocPath,
                            obj.companyID.ToString(),
                            obj.companyEDoc,
                            obj.companyEDocExtenstion);
                    }
                    
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

        [HttpPost("deleteCompany")]
        public IActionResult deleteCompany(CompanyCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";


                cmd2 = "UPDATE public.tbl_company SET \"isDeleted\"=1 where \"companyID\"="+obj.companyID+"";
            
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