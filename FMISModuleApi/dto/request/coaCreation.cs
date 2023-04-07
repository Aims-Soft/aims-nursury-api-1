using System;
using System.Text.Json.Serialization;


namespace FMISModuleApi.Entities
{
    public class COACreation
    {
        public int coaID { get; set; }
        public int coaTypeID { get; set; }
        public string coaTitle { get; set; }
        public int userID { get; set; }
        public int companyid { get; set; }
        public int businessid { get; set; }
    }
}