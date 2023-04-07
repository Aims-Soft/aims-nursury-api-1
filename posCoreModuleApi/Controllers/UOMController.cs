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
        private string cmd;

        public UOMController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getMeasurementUnit")]
        public IActionResult getMeasurementUnit()
        {
            try
            {
                cmd = "select * from public.\"measurementUnit\" where \"isDeleted\"::int = 0 ";
                var appMenu = dapperQuery.Qry<MeasurementUnit>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

    }
}