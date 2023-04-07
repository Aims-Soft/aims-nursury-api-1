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
    public class CityController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2;

        public CityController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getCity")]
        public IActionResult getCity()
        {
            try
            {
                cmd = "select * from public.\"city\" where \"isDeleted\"::int = 0 ";
                var appMenu = dapperQuery.Qry<City>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveCity")]
        public IActionResult saveCity(CityCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");
                var found = false;
                var city = "";

                List<City> appMenuCity = new List<City>();
                cmd2 = "select \"cityName\" from city where \"isDeleted\"::int = 0 and \"cityName\" = '" + obj.cityName + "'";
                appMenuCity = (List<City>)dapperQuery.QryResult<City>(cmd2, _dbCon);

                if (appMenuCity.Count > 0)
                    city = appMenuCity[0].cityName;

                int rowAffected = 0;
                var response = "";

                if (obj.cityID == 0)
                {
                    if (city == "")
                    {
                        cmd = "insert into public.city (\"cityName\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.cityName + "', '" + curDate + "', " + obj.userID + ", B'0')";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.city set \"cityName\" = '" + obj.cityName + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"cityID\" = " + obj.cityID + ";";
                }

                if (found == false)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
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

        [HttpPost("deleteCity")]
        public IActionResult deleteCity(CityCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.city set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"cityID\" = " + obj.cityID + ";";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
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

    }
}