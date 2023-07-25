using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class COA
    {
        public int coaID { get; set; }
        public int coaTypeID { get; set; }
        public string coaTitle { get; set; }
        public string coaTypeName { get; set; }
        public int isDeleted { get; set; }
        public int companyid { get; set; }
        public int businessid { get; set; }
    }
}