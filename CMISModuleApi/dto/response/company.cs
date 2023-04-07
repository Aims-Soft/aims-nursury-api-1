using System;
using System.Text.Json.Serialization;


namespace CMISModuleApi.Entities
{
    public class Company
    {
        public int companyID { get; set; }
        public string companyFullName { get; set; }
        public string companyAddress { get; set; }
        public string companyNtn { get; set; }
        public string companyStrn { get; set; }
        public string companyRegistrationNo { get; set; }
        public string companyShortName { get; set; }
        public string email { get; set; }
        public string mobileNo { get; set; }
        public string phoneNo { get; set; }
        public string companyEDoc { get; set; }
    }
}