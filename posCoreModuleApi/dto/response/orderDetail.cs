using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class OrderDetail
    {
        public int orderDetailID { get; set; }
        public int orderID { get; set; }
        public int productID { get; set; }
        public string productName { get; set; }
        public float price { get; set; }
        public float costPrice { get; set; }
        public int qty { get; set; }
        public int userID { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public int branchid { get; set; }
    }
}