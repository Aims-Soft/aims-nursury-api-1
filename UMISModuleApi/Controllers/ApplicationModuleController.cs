using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMISModuleAPI.Models;
using UMISModuleAPI.Services;
using UMISModuleAPI.Entities;
using Microsoft.Extensions.Options;
using UMISModuleAPI.Configuration;

namespace UMISModuleAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ApplicationModuleController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd;

        public ApplicationModuleController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getMenu")]
        public IActionResult getMenu(int roleId, int moduleId)
        {
            try
            {
                cmd = "select distinct \"menuId\", \"menuTitle\", \"parentRoute\", \"routeTitle\", \"read\", \"write\", \"delete\", \"parentMenuId\", \"menuSeq\" from view_user_role_details where \"roleID\" = " + roleId + " AND \"applicationModuleID\" = " + moduleId + " order by \"menuSeq\" Asc";
                var appMenu = dapperQuery.Qry<Menu>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getUserModules")]
        public IActionResult getUserModules(int roleId, int userId)
        {
            try
            {
                cmd = "select Distinct \"applicationModuleID\", \"applicationModuleTitle\", \"moduleIcon\" from view_user_role_details where \"roleID\" = " + roleId + " AND \"userId\" = " + userId + ";";
                var appMenu = dapperQuery.Qry<Menu>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

    }
}