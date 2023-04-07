using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
// using System.Data;
// using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using gatewayapi.Configuration;
using gatewayapi.Entities;
using gatewayapi.Helpers;
using gatewayapi.Models;
// using Dapper;
using gatewayapi.Services;

namespace gatewayapi.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        // IEnumerable<UserLogin> GetAll();

        UserLogin GetById(int id);

    }

    public class UserService : IUserService
    {

        private readonly IConfiguration _config;
        private readonly string dbCon;

        // private List<User> _users = new List<User>();

        private readonly JwtConfig _jwtConfig;

        private string cmd;

        public UserService(IOptions<JwtConfig> jwtConfig, IConfiguration config)
        {
            _jwtConfig = jwtConfig.Value;
            _config = config;
            dbCon = _config["conStr:user"];
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            // var user = _users.SingleOrDefault(x => x.userName == model.userName);
            // cmd = "select UserID AS id, EmpName AS fullName, loginName AS loginName, userType AS userType, region as region from logins where loginName = '" + model.userName + "' and password = '" + model.password + "'";
            cmd = "Select \"userID\" as userLoginId, \"loginName\" as loginName, '' as Password, \"roleId\", \"empName\" as FullName, \"isPinCode\", \"branchID\", \"companyID\", \"businessID\" from view_user_login where \"loginName\" = '" + model.Loginname + "' and \"Password\" = '" + model.hashpassword + "'";
            
            List<User> user = new List<User>(pgQuery.Qry<User>(cmd, dbCon));
            // using (IDbConnection con = new SqlConnection(dbCon))
            // {
            //     if (con.State == ConnectionState.Closed)
            //         con.Open();

            //     user = con.Query<User>("select UserID AS id, EmpName AS fullName, loginName AS loginName, userType AS userType, region as region from logins where loginName = '" + model.userName + "' and password = '" + model.password + "'").ToList();

            // }
            // return null if user not found
            if (user.Count == 0) return null;

            // authentaication successfull so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        // public AuthenticateResponse Authenticate(AuthenticateRequest model)
        // {
        //     // cmd = "Select UserID as userLoginId, LoginName as loginName, '' as Password, roleId, EmpName as FullName from view_user_login where loginName = '" + model.Loginname + "' and password = '" + model.hashpassword + "';";
        //     cmd = @"Select 'userID' as userLoginId, 'loginName' as loginName, '' as Password, 'roleId', 'empName' as FullName 
        //             from view_user_login 
        //             where 'loginName' = '" + model.userName + "' and 'Password' = '" + model.password + "';";

        //     List<User> user = new List<User>(pgQuery.Qry<User>(cmd, dbCon));

        //     if (user.Count == 0) return null;

        //     var token = generateJwtToken(user);

        //     return new AuthenticateResponse(user, token);
        // }

        // public IEnumerable<UserLogin> GetAll()
        // {
        //     // return _users;

        //     cmd = "select UserID AS id, EmpName AS fullName, loginName AS loginName, userType AS userType, region as region from logins";
        //     return pgQuery.Qry<UserLogin>(cmd, dbCon);
        // }

        public UserLogin GetById(int id)
        {
            // return _users.FirstOrDefault(x => x.id == id);

            cmd = "select UserID AS id, EmpName AS fullName, loginName AS loginName, userType AS userType, region as region from logins where userId = '" + id + "'";
            return pgQuery.Qry<UserLogin>(cmd, dbCon).FirstOrDefault();
        }

        // helper method
        private string generateJwtToken(List<User> user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userLoginId", user[0].userLoginId.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}