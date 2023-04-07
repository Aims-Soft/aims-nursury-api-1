using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMISModuleAPI.Models;
using UMISModuleAPI.Configuration;
using UMISModuleApi.Entities;
using UMISModuleAPI.Entities;
using UMISModuleAPI.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UMISModuleApi.dto.response;
using Newtonsoft.Json;
using MimeKit;
using System.Text;
using MailKit;
using Zaabee.SmtpClient;
using Dapper;
using Npgsql;
using System.Data;
using System.Collections.Generic;
using System.Net.Mail;

namespace UMISModuleAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private string cmd,cmd2,cmd3,cmd4;
        private string randomNumber;
        private readonly IOptions<conStr> _dbCon;

        private readonly JwtConfig _jwtConfig;

        public UserController(IOptions<JwtConfig> jwtConfig, IUserService userService, IOptions<conStr> dbCon)
        {
            _jwtConfig = jwtConfig.Value;
            _userService = userService;
            _dbCon = dbCon;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            if (response == null)
                return BadRequest(new { message = "user name or password is incorrect" });

            return Ok(response);
        }

        [HttpGet("getPin")]
        public IActionResult getPin(int pin, int userID)
        {
            try
            {
                cmd = "SELECT \"userID\" FROM users WHERE \"isDeleted\" = 0 And \"userID\" = '" + userID + "' and pincode = '" + pin + "';";
                var appMenu = dapperQuery.Qry<User>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getPassword")]
        public IActionResult getPassword(int userID)
        {
            try
            {
                cmd = "SELECT \"userID\",\"Password\" FROM users where \"userID\" = "+userID+" ";
                var appMenu = dapperQuery.Qry<User>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getUserAddress")]
        public IActionResult getUserAddress(int userID)
        {
            try
            {
                cmd = "SELECT \"userAddressID\",\"userID\",\"city\",\"address\",\"area\",\"label\" FROM user_address where \"userID\" = " + userID + " and \"isDeleted\"=0";
                var appMenu = dapperQuery.Qry<UserAddress>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getUserDetail")]
        public IActionResult getUserDetail(int userID)
        {
            try
            {
                cmd = "SELECT \"userID\",\"empName\",\"loginName\",\"Password\",\"dateOfBirth\",\"gender\",\"applicationEDoc\" FROM users WHERE \"isDeleted\" = 0 And \"userID\" = " +userID + ";";
                var appMenu = dapperQuery.Qry<UserDetail>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getVerifyOTP")]
        public IActionResult getVerifyOTP(string OTP)
        {
            try
            {

                cmd = "SELECT \"otpNo\" from \"OTP\" where \"otpNo\" = '" + OTP + "' ";
                var appMenu = dapperQuery.Qry<OTP>(cmd, _dbCon);

                return Ok(appMenu);    
                
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("saveOTP")]
        public IActionResult saveOTP(SendEmail obj)
        {
            Random rnd = new Random();
            randomNumber = (rnd.Next(1000,9999)).ToString();
            
            int rowAffected = 0;
            var response = "";
            var newOTPid = 0;

            List<OTP> appMenuUserID = new List<OTP>();
            cmd3 = "select \"otpID\" from public.\"OTP\" ORDER BY \"otpID\" DESC LIMIT 1";
            appMenuUserID = (List<OTP>)dapperQuery.Qry<OTP>(cmd3, _dbCon);

            if(appMenuUserID.Count == 0)
                {
                    newOTPid = 1;    
                }else{
                    newOTPid = appMenuUserID[0].otpID+1;
                }

            cmd2 = "insert into public.\"OTP\" (\"otpID\",\"otpNo\",\"timestamp\") values (" + newOTPid + ",'" + randomNumber + "',current_timestamp)";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd2);
                }

                if (rowAffected > 0)
                {   
                    
                    using (MailMessage mail = new MailMessage())
                        {
                            mail.From = new MailAddress("noreply@mysite.com");
                            mail.To.Add(obj.userEmail);
                            mail.Subject = "Your one time password for verification";
                            mail.Body = "your one time password is "+randomNumber+".";
                            mail.IsBodyHtml = true;

                            //* for setting smtp mail name and port
                            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                            {
                                smtp.UseDefaultCredentials = false;
                                //* for setting sender credentials(email and password) using smtp
                                smtp.Credentials = new System.Net.NetworkCredential("hammadhere12@gmail.com","bicmzfivilxrhsla");
                                smtp.EnableSsl = true;
                                
                                smtp.Send(mail);
                            }
                        }
                        
                    response = "Mail Sent!";
                    return Ok(new { message = response });

                }
                else
                {
                    
                    response = "Something went wrong";
                    return BadRequest(new { message = response });

                }

                
        }

        [HttpGet("getUser")]
        public IActionResult getUser(int companyID, int businessID)
        {
            try
            {   
                if (companyID == 0 && businessID == 0)
                {
                    cmd = "SELECT * FROM \"view_allUser\"";
                }
                else
                {
                    cmd = "SELECT * FROM \"view_allUser\" where \"companyID\" = " + companyID + " AND \"businessID\" = " + businessID + ";";    
                }
                
                var appMenu = dapperQuery.Qry<UserDetail>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveUser")]
        public IActionResult saveUser(UserCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                var found = false;
                var userName = "";
                var newUserID = 0;
                var newUserRoleID = 0;


                List<UserCreation> appMenuUser = new List<UserCreation>();
                cmd = "select \"loginName\" from users where \"loginName\"='" + obj.loginName + "' AND \"branchID\" = " + obj.branchID + "";
                appMenuUser = (List<UserCreation>)dapperQuery.Qry<UserCreation>(cmd, _dbCon);

                List<UserCreation> appMenuUserID = new List<UserCreation>();
                cmd3 = "select \"userID\" from users ORDER BY \"userID\" DESC LIMIT 1";
                appMenuUserID = (List<UserCreation>)dapperQuery.Qry<UserCreation>(cmd3, _dbCon);
                if(appMenuUserID.Count == 0)
                    {
                        newUserID = 1;    
                    }else{
                        newUserID = appMenuUserID[0].userID+1;
                    }

                List<UserRoleCreation> appMenuUserRoleID = new List<UserRoleCreation>();
                cmd3 = "select \"userRoleId\" from user_roles ORDER BY \"userRoleId\" DESC LIMIT 1";
                appMenuUserRoleID = (List<UserRoleCreation>)dapperQuery.Qry<UserRoleCreation>(cmd3, _dbCon);
                if(appMenuUserRoleID.Count == 0)
                    {
                        newUserRoleID = 1;    
                    }else{
                        newUserRoleID = appMenuUserRoleID[0].userRoleId+1;
                    }

                
                if (appMenuUser.Count > 0)
                        userName = appMenuUser[0].loginName;

                if(userName=="")
                {
                    if (obj.userTypeID == 1)
                    {
                        cmd2 = "insert into public.\"users\" (\"userID\",\"empName\", \"loginName\", \"Password\", \"dateOfBirth\", \"gender\", \"createdOn\", \"isDeleted\", \"userTypeID\", \"branchID\", \"companyID\", \"businessID\") values ("+newUserID+",'" + obj.empName + "','" + obj.loginName + "','" + obj.Password + "', '" + obj.dateOfBirth + "', '" + obj.gender + "' ,'" + curDate + "', 0, " + obj.userTypeID + ", " + obj.branchID + ", " + obj.companyID + ", " + obj.businessID + ")";
                    }
                    else
                    {
                        cmd2 = "insert into public.\"users\" (\"userID\",\"empName\", \"loginName\", \"Password\", \"dateOfBirth\", \"gender\", \"createdOn\", \"isDeleted\", \"userTypeID\" \"branchID\", \"companyID\", \"businessID\") values ("+newUserID+",'" + obj.empName + "','" + obj.loginName + "','" + obj.Password + "',1,'" + obj.dateOfBirth + "','" + obj.gender + "', '" + curDate + "', 0, " + obj.userTypeID + ", " + obj.branchID + ", " + obj.companyID + ", " + obj.businessID + ")";    
                    }
                    
                    //cmd4 = "insert into public.\"user_roles\" (\"userRoleId\",\"roleId\", \"userId\",\"createdOn\", \"isDeleted\") VALUES ('"+newRoleID+"',1,'" +newUserID+ "','" + curDate + "', 0)";
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
                    if (obj.userTypeID == 1)
                    {
                        cmd4 = "insert into public.\"user_roles\" (\"userRoleId\",\"roleId\", \"userId\",\"createdOn\", \"isDeleted\") VALUES ("+newUserRoleID+"," + obj.roleId + "," + newUserID + ",'" + curDate + "', 0)";    
                    }
                    else
                    {
                        cmd4 = "insert into public.\"user_roles\" (\"userRoleId\",\"roleId\", \"userId\",\"createdOn\", \"isDeleted\") VALUES ("+newUserRoleID+",1," + newUserID + ",'" + curDate + "', 0)";
                    }
                    
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected2 = con.Execute(cmd4);
                    }

                    if (rowAffected > 0 && rowAffected2 > 0)
                    {
                        response = "Success";
                        return Ok(new { message = response });
                    }
                    else
                    {
                        response = "Server Issue";
                        return Ok(new { message = response });
                    }
                    

                }
                else
                {
                    if (found == true)
                    {
                        response = "Login name already exist";
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

        [HttpPost("updateUser")]
        public IActionResult updateUser(UserCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                // var newUserID = 0;
                // var newRoleID = 0;


                // List<userCreation> appMenuUser = new List<userCreation>();
                // cmd = "select \"loginName\" from users where \"loginName\"='" + obj.loginName + "'";
                // appMenuUser = (List<userCreation>)dapperQuery.QryResult<userCreation>(cmd, _dbCon);
                if (obj.userTypeID == 1)
                {
                    cmd = "update public.\"users\" set \"empName\" = '" + obj.empName + "', \"loginName\" = '" + obj.loginName +"' ,\"dateOfBirth\" = '"+ obj.dateOfBirth +"', \"gender\" = '"+ obj.gender +"', \"companyID\" = '"+ obj.companyID +"', \"businessID\" = '"+ obj.businessID +"', \"branchID\" = '"+ obj.branchID +"', \"modifiedOn\" = '" + curDate + "' where \"userID\"="+obj.userID+"";    
                }
                else
                {
                    cmd = "update public.\"users\" set \"empName\" = '" + obj.empName + "', \"companyID\" = '"+ obj.companyID +"', \"businessID\" = '"+ obj.businessID +"', \"branchID\" = '"+ obj.branchID +"', \"loginName\" = '" + obj.loginName +"' ,\"dateOfBirth\" = '"+ obj.dateOfBirth +"',\"gender\" = '"+ obj.gender +"', \"modifiedOn\" = '" + curDate + "', \"applicationEDoc\" = '" + obj.applicationEDocPath + "' where \"userID\"="+obj.userID+"";
                }
                
                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                if (rowAffected > 0)
                {
                    
                    if (obj.userTypeID == 1)
                    {
                        cmd2 = "update public.\"user_roles\" SET \"modifiedOn\"='"+curDate+"',\"roleId\"=" + obj.roleId + ", \"modifiedBy\"="+obj.userID+" where \"userId\"="+obj.userID+"";    
                    }
                    else
                    {
                        cmd2 = "update public.\"user_roles\" SET \"modifiedOn\"='"+curDate+"', \"modifiedBy\"="+obj.userID+" where \"userId\"="+obj.userID+"";
                    }

                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected2 = con.Execute(cmd2);
                    }

                    if (obj.applicationEDocPath != null && obj.applicationEDocPath != "")
                    {
                        dapperQuery.saveImageFile(
                            obj.applicationEDocPath,
                            obj.userID.ToString(),
                            obj.applicationEDoc,
                            obj.applicationEdocExtenstion);
                    }
                    
                }

                if (rowAffected > 0 && rowAffected2 > 0)
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

        [HttpPost("deleteUser")]
        public IActionResult deleteUser(UserCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";


                List<UserCreation> appMenuAddressID = new List<UserCreation>();
                cmd3 = "select \"userAddressID\" from user_address ORDER BY \"userAddressID\" DESC LIMIT 1";
                appMenuAddressID = (List<UserCreation>)dapperQuery.Qry<UserCreation>(cmd3, _dbCon);
                

                cmd2 = "UPDATE public.\"users\" SET \"isDeleted\"=1 where \"userID\"="+obj.userID+"";
                cmd3 = "UPDATE public.\"user_roles\" SET \"isDeleted\"=1 where \"userId\"="+obj.userID+"";
            
                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd2);
                }

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected2 = con.Execute(cmd3);
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

        [HttpPost("updatePassword")]
        public IActionResult updatePassword(UserCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                


                List<UserCreation> appMenuUserID = new List<UserCreation>();
                cmd3 = "select \"userID\" from users ORDER BY \"userID\" DESC LIMIT 1";
                appMenuUserID = (List<UserCreation>)dapperQuery.Qry<UserCreation>(cmd3, _dbCon);
                
                cmd2 = "UPDATE public.\"users\" SET \"Password\"='" + obj.Password + "' where \"userID\"="+obj.userID+"";
            
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

        [HttpPost("forgetPassword")]
        public IActionResult forgetPassword(UserCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                
                List<UserCreation> appMenuUserID = new List<UserCreation>();
                cmd3 = "select \"userID\" from users ORDER BY \"userID\" DESC LIMIT 1";
                appMenuUserID = (List<UserCreation>)dapperQuery.Qry<UserCreation>(cmd3, _dbCon);
                
                cmd2 = "UPDATE public.\"users\" SET \"Password\"='"+obj.Password+"' where \"loginName\"='"+obj.loginName+"'";
            
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
                    
                    response = "please try again later";
                    
                return BadRequest(new { message = response });

                }

            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveUserAddress")]
        public IActionResult saveUserAddress(UserAddressCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                var found = false;
                var userAddress = "";
                var newUserAddressID  = 0;

                List<UserAddressCreation> appMenuAddress = new List<UserAddressCreation>();
                cmd = "select \"address\" from user_address where \"address\"='" + obj.address + "' and \"userID\" = " + obj.userID + " and \"isDeleted\" = 0";
                appMenuAddress = (List<UserAddressCreation>)dapperQuery.Qry<UserAddressCreation>(cmd, _dbCon);

                List<UserAddressCreation> appMenuAddressID = new List<UserAddressCreation>();
                cmd3 = "select \"userAddressID\" from user_address ORDER BY \"userAddressID\" DESC LIMIT 1";
                appMenuAddressID = (List<UserAddressCreation>)dapperQuery.Qry<UserAddressCreation>(cmd3, _dbCon);
                
                if(appMenuAddressID.Count<=0)
                {
                    newUserAddressID = 1;
                }
                else
                {
                    newUserAddressID = appMenuAddressID[0].userAddressID+1;
                }

                if (appMenuAddress.Count > 0)
                        userAddress = appMenuAddress[0].address;

                if(userAddress=="")
                {
                    cmd2 = "insert into public.\"user_address\" (\"userAddressID\",\"userID\", \"city\", \"address\", \"isDeleted\", \"area\", \"label\") values ('"+newUserAddressID+"','" + obj.userID + "','" + obj.city + "','" + obj.address + "',0,'" + obj.area + "','" + obj.label + "')";
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

                if (rowAffected > 0)
                {
                    response = "Success";
                return Ok(new { message = response });

                }
                else
                {
                    if (found == true)
                    {
                        response = "Address already exist";
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

        [HttpPost("updateUserAddress")]
        public IActionResult updateUserAddress(UserAddressCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";


                List<UserAddressCreation> appMenuAddressID = new List<UserAddressCreation>();
                cmd3 = "select \"userAddressID\" from user_address ORDER BY \"userAddressID\" DESC LIMIT 1";
                appMenuAddressID = (List<UserAddressCreation>)dapperQuery.Qry<UserAddressCreation>(cmd3, _dbCon);
                
                cmd2 = "UPDATE public.\"user_address\" SET \"city\"='"+obj.city+"', \"address\"='"+obj.address+"', \"area\"='"+obj.area+"', \"label\"='"+obj.label+"' where \"userAddressID\"="+obj.userAddressID+" and \"userID\"="+obj.userID+"";
            
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
                    
                    response = "result not update";
                    
                return BadRequest(new { message = response });

                }

            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("deleteUserAddress")]
        public IActionResult deleteUserAddress(UserAddressCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";


                List<UserAddressCreation> appMenuAddressID = new List<UserAddressCreation>();
                cmd3 = "select \"userAddressID\" from user_address ORDER BY \"userAddressID\" DESC LIMIT 1";
                appMenuAddressID = (List<UserAddressCreation>)dapperQuery.Qry<UserAddressCreation>(cmd3, _dbCon);
                
                cmd2 = "UPDATE public.\"user_address\" SET \"isDeleted\"=1 where \"userAddressID\"="+obj.userAddressID+" and \"userID\"="+obj.userID+"";
            
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

        [HttpGet("genToken")]
        public IActionResult genToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userLoginId", "1") }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescription);
            return Ok(new {message = tokenHandler.WriteToken(token)});
        }
        
        [HttpGet("getTestData")]
        public IActionResult getTestData()
        {
            return Ok("OK");
        }
    }
}