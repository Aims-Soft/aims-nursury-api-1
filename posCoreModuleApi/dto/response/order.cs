using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Order
    {
        public int orderID { get; set; }
        public string orderDate { get; set; }
        public string customerName { get; set; }
        public string status { get; set; }
        public string orderType { get; set; }
        public string total { get; set; }
        public int userID { get; set; }
        public string json { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int branchid { get; set; }
        public int moduleId { get; set; }
    }
}