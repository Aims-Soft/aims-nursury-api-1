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
    public class UOMController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd;
        public string saveConStr;

        public UOMController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getMeasurementUnit")]
        public IActionResult getMeasurementUnit(int businessID,int userID, int moduleId)
        {
            try
            {
                cmd = "select * from public.\"measurementUnit\" where \"isDeleted\"::int = 0  and \"businessid\" = " + businessID + "";
                var appMenu = _dapperQuery.StrConQry<MeasurementUnit>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

    }
}