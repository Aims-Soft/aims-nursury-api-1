using System;
using System.Text.Json.Serialization;


namespace CMISModuleApi.Entities
{
    public class Business
    {
        public int companyID { get; set; }
        public int businessID { get; set; }
        public string companyFullName { get; set; }
        public string businessFullName { get; set; }
        public string businessAddress { get; set; }
        public string businessShortName { get; set; }
        public string email { get; set; }
        public string mobileNo { get; set; }
        public string phoneNo { get; set; }
        public string businessEDoc { get; set; }
    }
}