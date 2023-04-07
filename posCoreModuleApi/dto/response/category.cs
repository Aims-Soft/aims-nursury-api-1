using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Category
    {
        public int categoryID { get; set; }
        public string categoryName { get; set; }
        public int parentCategoryID { get; set; }
        public string level1 { get; set; }
        public string level2 { get; set; }
        public string level3 { get; set; }
        public string level4 { get; set; }
        public string level5 { get; set; }
        public int isDeleted { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}