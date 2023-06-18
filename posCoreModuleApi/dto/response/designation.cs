using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Designation
    {
        public int designationID { get; set; }
        public string desginationName { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int branchID { get; set; }
    }
}