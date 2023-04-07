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
    public class LocationController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd;

        public LocationController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getLocation")]
        public IActionResult getLocation(int businessid,int companyid)
        {
            try
            {
                cmd = "select * from public.\"location\" where \"isDeleted\"::int = 0 and \"parentLocationID\" is not null AND \"businessid\" = " + businessid + " AND \"companyid\" = " + companyid + "";
                var appMenu = dapperQuery.Qry<Location>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

    }
}