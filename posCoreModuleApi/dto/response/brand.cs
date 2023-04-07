using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Brand
    {
        public int brandID { get; set; }
        public string brandName { get; set; }
        public string description { get; set; }
        public int isDeleted { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}