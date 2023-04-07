using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMISModuleApi.Entities
{
    public class BusinessCreation
    {
        public int companyID { get; set; }
        public int businessID { get; set; }
        public string businessFullName { get; set; }
        public string businessAddress { get; set; }
        public string businessShortName { get; set; }
        public string email { get; set; }
        public string mobileNo { get; set; }
        public string phoneNo { get; set; }
        public string businessEDoc { get; set; }    
        public string businessEDocPath { get; set; }
        public string businessEDocExtenstion { get; set; }
        public int userID { get; set; }
    }
}