using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class DesignationCreation
    {
        public int designationID { get; set; }
        public string designationName { get; set; }
        public int userID { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}