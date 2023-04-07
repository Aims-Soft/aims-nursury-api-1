using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMISModuleApi.Entities
{
    public class UserCreation
    {
        public int userID { get; set; }
        public string empName { get; set; }
        public string loginName { get; set; }
        public string Password { get; set; }
        public int outletid { get; set; }
        public int roleId { get; set; }
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
        public string applicationEDocPath { get; set; }
        public string applicationEDoc { get; set; }
        public string applicationEdocExtenstion { get; set; }
        public int userTypeID { get; set; }
        public int companyID { get; set; }
        public int businessID { get; set; }
        public int branchID { get; set; }
    }
}