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

namespace posCoreModuleApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2;
        public string saveConStr;

        public CategoryController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getCategory")]
        public IActionResult getCategory(int branchID,int companyID,int userID, int moduleId)
        {
            try
            {
                if(companyID == 0 && branchID != 0){
                    cmd = "select * from public.\"category\" where \"isDeleted\"::int = 0 AND \"branchID\" = " + branchID + "  and \"parentCategoryID\" is null";
                }else{
                    cmd = "select * from public.\"category\" where \"isDeleted\"::int = 0 and \"parentCategoryID\" is null AND \"branchID\" = " + branchID + " AND \"companyid\" = " + companyID + "";
                }
                var appMenu = _dapperQuery.StrConQry<Category>(cmd,userID,moduleId); 
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getSubCategory")]
        public IActionResult getSubCategory(int catID, int companyID, int branchID,int userID, int moduleId)
        {
            try
            {
                if(companyID == 0 && branchID != 0){
                    if (catID == 0)
                    {
                        cmd = "select * from public.\"category\" where \"isDeleted\"::int = 0 AND \"branchID\" = " + branchID + " and \"parentCategoryID\"  is not null";
                    }
                    else
                    {
                        cmd = "select * from public.\"category\" where \"isDeleted\"::int = 0 AND \"branchID\" = " + branchID + " and \"parentCategoryID\" = " + catID + ";";
                    }
                }else{
                    if (catID == 0)
                    {
                        cmd = "select * from public.\"category\" where companyid = "+ companyID +" AND \"branchID\" = " + branchID + " AND \"isDeleted\"::int = 0 and \"parentCategoryID\"  is not null";
                    }
                    else
                    {
                        cmd = "select * from public.\"category\" where companyid = "+ companyID +" AND \"branchID\" = " + branchID + " AND \"isDeleted\"::int = 0 and \"parentCategoryID\" = " + catID + ";";
                    }
                }
                var appMenu = _dapperQuery.StrConQry<Category>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveCategory")]
        public IActionResult saveCategory(CategoryCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                var found = false;
                var category = "";
                var newCategoryID = 0;

                List<Category> appMenuCategory = new List<Category>();
                cmd2 = "select \"categoryName\" from category where \"isDeleted\"::int = 0 and \"parentCategoryID\" is null and \"categoryName\" = '" + obj.categoryName + "' AND \"businessid\" = " + obj.businessID + " AND \"companyid\" = " + obj.companyID + "";
                appMenuCategory = (List<Category>)_dapperQuery.StrConQry<Category>(cmd2, obj.userID,obj.moduleId);

                if (appMenuCategory.Count > 0)
                    category = appMenuCategory[0].categoryName;

                List<Category> appMenuShift = new List<Category>();
                cmd = "select \"categoryID\" from category ORDER BY \"categoryID\" DESC LIMIT 1";
                appMenuShift = (List<Category>)_dapperQuery.StrConQry<Category>(cmd, obj.userID,obj.moduleId);

                    if (appMenuShift.Count > 0)
                    {
                        newCategoryID = appMenuShift[0].categoryID + 1;
                    }
                    else
                    {
                        newCategoryID = 1;
                    }

                if (obj.categoryID == 0)
                {
                    if (category == "")
                    {
                        cmd = "insert into public.category (\"categoryID\",\"categoryName\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\",\"branchID\") values ("+ newCategoryID +",'" + obj.categoryName + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessID + "," + obj.companyID + "," + obj.branchID + ")";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.category set \"categoryName\" = '" + obj.categoryName + "', \"modifiefOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"categoryID\" = " + obj.categoryID + ";";
                }

                if (found == false)
                {
                    if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
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
                    if (found == true)
                    {
                        response = "Record already exist";
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                }

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }

        }

        [HttpPost("deleteCategory")]
        public IActionResult deleteCategory(CategoryCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.category set \"isDeleted\" = B'1', \"modifiefOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"categoryID\" = " + obj.categoryID + ";";

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
                    response = "Server Issue";
                }

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveSubCategory")]
        public IActionResult saveSubCategory(CategoryCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                var found = false;
                var category = "";
                var newCategoryID = 0;

                List<Category> appMenuCategory = new List<Category>();
                cmd2 = "select \"categoryName\" from category where \"isDeleted\"::int = 0 and \"parentCategoryID\" ='" + obj.parentCategoryID + "' and \"categoryName\" = '" + obj.categoryName + "'";
                appMenuCategory = (List<Category>)_dapperQuery.StrConQry<Category>(cmd2, obj.userID,obj.moduleId);

                if (appMenuCategory.Count > 0)
                    category = appMenuCategory[0].categoryName;

                List<Category> appMenuShift = new List<Category>();
                cmd = "select \"categoryID\" from category ORDER BY \"categoryID\" DESC LIMIT 1";
                appMenuShift = (List<Category>)_dapperQuery.StrConQry<Category>(cmd, obj.userID,obj.moduleId);

                    if (appMenuShift.Count > 0)
                    {
                        newCategoryID = appMenuShift[0].categoryID + 1;
                    }
                    else
                    {
                        newCategoryID = 1;
                    }

                if (obj.categoryID == 0)
                {
                    if (category == "")
                    {
                        cmd = "insert into public.category (\"categoryID\",\"categoryName\", \"parentCategoryID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\",\"branchID\") values ("+ newCategoryID +",'" + obj.categoryName + "', '" + obj.parentCategoryID + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessID + "," + obj.companyID + "," + obj.branchID + ")";

                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.category set \"categoryName\" = '" + obj.categoryName + "', \"parentCategoryID\" = '" + obj.parentCategoryID + "', \"modifiefOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"categoryID\" = " + obj.categoryID + ";";
                }

                if (found == false)
                {
                    if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
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
                    if (found == true)
                    {
                        response = "Record already exist";
                    }
                    else
                    {
                        response = "Server Issue";
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