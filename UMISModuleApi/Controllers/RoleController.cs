using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UMISModuleAPI.Configuration;
using UMISModuleApi.Entities;
using UMISModuleAPI.Entities;
using UMISModuleAPI.Services;
using Dapper;
using System.Data;
using Npgsql;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UMISModuleApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private IUserService _userService;
        private string cmd,cmd2,cmd3,cmd4,cmd5,cmd6;
        private readonly IOptions<conStr> _dbCon;

        public RoleController(IUserService userService, IOptions<conStr> dbCon)
        {
            _userService = userService;
            _dbCon = dbCon;
        }
        
        [HttpGet("getApplicationMenu")]
        public IActionResult getApplicationMenu()
        {
            try
            {
                cmd = "SELECT \"applicationModuleID\",\"applicationModuleTitle\" from \"application_modules\" where \"isDeleted\" = 0";
                var appMenu = dapperQuery.Qry<ApplicationModule>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getRoles")]
        public IActionResult getRoles(int businessID)
        {
            try
            {
                if(businessID == 0){
                    cmd = "SELECT * from \"roles\" where \"isDeleted\" = 0";
                }else{
                    cmd = "SELECT * from \"roles\" where \"isDeleted\" = 0 AND \"busniessID\" = " + businessID + "";
                }
                var appMenu = dapperQuery.Qry<Roles>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getMenuRoleByModuleId")]
        public IActionResult getMenuRoleByModuleId(int applicationModuleID)
        {
            try
            {
                if(applicationModuleID == 0){
                    cmd = "SELECT * from \"view_role_detail\"";
                }else{
                    cmd = "SELECT * from \"view_role_detail\" where \"applicationModuleId\"= " + applicationModuleID + " ";
                }
                var appMenu = dapperQuery.Qry<Menu>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getMenu")]
        public IActionResult getMenu(int roleID)
        {
            try
            {
                cmd = "SELECT * from \"view_allMenu\" where \"roleId\" = " + roleID + "";
                
                var appMenu = dapperQuery.Qry<Menu>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getMenuList")]
        public IActionResult getMenuList(int roleID)
        {
            try
            {
                // cmd = "SELECT \"menuId\",\"menuTitle\" from view_menu";
                cmd = "SELECT \"menuId\",\"menuTitle\" from \"viewMenuRoles\" where \"roleID\"= " + roleID + "";
                var appMenu = dapperQuery.Qry<Menu>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getRoleDetail")]
        public IActionResult getRoleDetail(int roleID)
        {
            try
            {
                cmd = "select j as json from \"fun_roleDetail\"(" + roleID + ")";
                var appMenu = dapperQuery.Qry<JsonList>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveRole")]
        public IActionResult saveRole(RoleCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                var found = false;
                var roleTitle = "";
                var newRoleID = 0;
                var newRoleDetailID = 0;

                List<RoleCreation> appMenuUser = new List<RoleCreation>();
                cmd = "select \"roleTitle\" from roles where \"roleTitle\"='" + obj.roleTitle + "' AND \"busniessID\" = " + obj.businessID + "";
                appMenuUser = (List<RoleCreation>)dapperQuery.Qry<RoleCreation>(cmd, _dbCon);

                List<RoleCreation> appMenuRoleID = new List<RoleCreation>();
                cmd3 = "select \"roleID\" from roles ORDER BY \"roleID\" DESC LIMIT 1";
                appMenuRoleID = (List<RoleCreation>)dapperQuery.Qry<RoleCreation>(cmd3, _dbCon);

                
                
                if(appMenuRoleID.Count == 0)
                    {
                        newRoleID = 1;    
                    }else{
                        newRoleID = appMenuRoleID[0].roleID+1;
                    }

                if (appMenuUser.Count > 0)
                        roleTitle = appMenuUser[0].roleTitle;

                
                if(roleTitle == "")
                {   

                    cmd2 = "insert into public.\"roles\" (\"roleID\", \"roleTitle\", \"roleDescription\", \"companyID\", \"busniessID\", \"createdBy\", \"createdOn\", \"isDeleted\") values (" + newRoleID + ", '" + obj.roleTitle + "', '" + obj.roleDescription + "', " + obj.companyID + ", " + obj.businessID + ", " + obj.userID + ", '" + curDate + "', 0)";
                    
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
                    var invObject = JsonConvert.DeserializeObject<List<RoleDetailCreation>>(obj.json);

                    foreach (var item in invObject)
                    {
                        List<RoleDetailCreation> appMenuRoleDetailID = new List<RoleDetailCreation>();
                        cmd4 = "select \"roleDetailId\" from roles_detail ORDER BY \"roleDetailId\" DESC LIMIT 1";
                        appMenuRoleDetailID = (List<RoleDetailCreation>)dapperQuery.Qry<RoleDetailCreation>(cmd4, _dbCon); 

                        if(appMenuRoleDetailID.Count == 0)
                        {
                            newRoleDetailID = 1;    
                        }else{
                            newRoleDetailID = appMenuRoleDetailID[0].roleDetailID+1;
                        }

                        cmd6 = "insert into public.\"roles_detail\" (\"roleDetailId\", \"menuId\", \"roleId\", \"createdOn\", \"createdBy\", \"isDeleted\", \"read\", \"write\", \"delete\") values (" + newRoleDetailID + ", '" + item.menuId + "', " + newRoleID + ", '" + curDate + "', " + obj.userID + ", 0," + Convert.ToInt32(item.read) + "," + Convert.ToInt32(item.write) + "," + Convert.ToInt32(item.delete) + ")";
                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd6);
                        }

                    }
                    

                }
                if(rowAffected > 0 && rowAffected2 > 0 )
                {
                    response = "Success";
                    return Ok(new { message = response });
                }
                else
                {
                    if (found == true)
                    {
                        response = "Role already exist";
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

        [HttpPost("updateRole")]
        public IActionResult updateRole(RoleCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                var found = false;
                var roleTitle = "";
                var newRoleID = 0;
                var newRoleDetailID = 0;
                

                List<RoleCreation> appMenuUser = new List<RoleCreation>();
                cmd = "select \"roleTitle\" from roles where \"roleTitle\"='" + obj.roleTitle + "' AND \"roleID\" <> " + obj.roleID + " AND \"busniessID\" = " + obj.businessID + "";
                appMenuUser = (List<RoleCreation>)dapperQuery.Qry<RoleCreation>(cmd, _dbCon);

                List<RoleCreation> appMenuRoleID = new List<RoleCreation>();
                cmd3 = "select \"roleID\" from roles ORDER BY \"roleID\" DESC LIMIT 1";
                appMenuRoleID = (List<RoleCreation>)dapperQuery.Qry<RoleCreation>(cmd3, _dbCon);

                
                
                if(appMenuRoleID.Count == 0)
                    {
                        newRoleID = 1;    
                    }else{
                        newRoleID = appMenuRoleID[0].roleID+1;
                    }

                if (appMenuUser.Count > 0)
                        roleTitle = appMenuUser[0].roleTitle;

                
                if(obj.roleID >= 1 && roleTitle == "")
                {   

                    cmd2 = "update public.\"roles\" set  \"roleTitle\" = '" + obj.roleTitle + "', \"roleDescription\" = '" + obj.roleDescription + "', \"companyID\" = '" + obj.companyID + "', \"businessID\" = '" + obj.businessID + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"roleID\" = " + obj.roleID + ";";
                    
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

                    cmd5 = "update public.\"roles_detail\" set \"isDeleted\" = 1 where \"roleId\" = " + obj.roleID + ";";
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected2 = con.Execute(cmd5);
                    }

                    var invObject = JsonConvert.DeserializeObject<List<RoleDetailCreation>>(obj.json);

                    foreach (var item in invObject)
                    {
                        List<RoleDetailCreation> appMenuRoleDetailID = new List<RoleDetailCreation>();
                        cmd4 = "select \"roleDetailId\" from roles_detail ORDER BY \"roleDetailId\" DESC LIMIT 1";
                        appMenuRoleDetailID = (List<RoleDetailCreation>)dapperQuery.Qry<RoleDetailCreation>(cmd4, _dbCon);

                        List<RoleDetailCreation> appMenuRoleMenuDetail = new List<RoleDetailCreation>();
                        cmd6 = "select \"roleDetailId\" from roles_detail where \"roleId\" = " + obj.roleID + " and \"menuId\" = " + item.menuId + " ";
                        appMenuRoleMenuDetail = (List<RoleDetailCreation>)dapperQuery.Qry<RoleDetailCreation>(cmd6, _dbCon);
                        


                        if (appMenuRoleMenuDetail.Count == 0)
                        {
                            if(appMenuRoleDetailID.Count == 0)
                            {
                                newRoleDetailID = 1;    
                            }else{
                                newRoleDetailID = appMenuRoleDetailID[0].roleDetailID+1;
                            }

                            cmd6 = "insert into public.\"roles_detail\" (\"roleDetailId\", \"menuId\", \"roleId\", \"createdOn\", \"createdBy\", \"isDeleted\", \"read\", \"write\", \"delete\") values (" + newRoleDetailID + ", '" + item.menuId + "', " + obj.roleID + ", '" + curDate + "', " + obj.userID + ", 0," + Convert.ToInt32(item.read) + "," + Convert.ToInt32(item.write) + "," + Convert.ToInt32(item.delete) + ")";
                            using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                            {
                                rowAffected2 = con.Execute(cmd6);
                            }
                        }
                        else
                        {
                            cmd = "update public.\"roles_detail\" set \"isDeleted\" = 0 where \"roleId\" = " + obj.roleID + " and \"menuId\" = " + item.menuId + " ";
                            using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                            {
                                rowAffected2 = con.Execute(cmd);
                            }
                        }
                    
                    }
                    

                }
                if(rowAffected > 0 )
                {
                    response = "Success";
                    return Ok(new { message = response });
                }
                else
                {
                    if (found == true)
                    {
                        response = "Role not exist";
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

        [HttpPost("deleteRole")]
        public IActionResult deleteRole(RoleCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                var found = false;

                
                if(obj.roleID >= 1)
                {   

                    cmd2 = "update public.\"roles\" set  \"roleTitle\" = '" + obj.roleTitle + "', \"roleDescription\" = '" + obj.roleDescription + "' , \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"roleID\" = " + obj.roleID + ";";
                    cmd5 = "update public.\"roles_detail\" set \"isDeleted\" = 1 where \"roleId\" = " + obj.roleID + ";";
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
                    
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected2 = con.Execute(cmd5);
                    }
                }
                
                if(rowAffected > 0 && rowAffected2 > 0)
                {
                    response = "Success";
                    return Ok(new { message = response });
                }
                else
                {
                    if (found == true)
                    {
                        response = "Role not exist";
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
        
    }
}