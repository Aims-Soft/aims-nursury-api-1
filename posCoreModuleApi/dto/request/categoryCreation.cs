using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class CategoryCreation
    {
        public int categoryID { get; set; }
        public int parentCategoryID { get; set; }
        public string categoryName { get; set; }
        public int userID { get; set; }
        public int businessID { get; set; }
        public int companyID { get; set; }
    }
}