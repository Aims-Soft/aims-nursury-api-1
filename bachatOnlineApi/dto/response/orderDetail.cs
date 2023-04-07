using System;
using System.Text.Json.Serialization;


namespace bachatOnlineModuleApi.Entities
{
    public class OrderDetail
    {
        public int orderDetailID { get; set; }
        public int orderID { get; set; }
        public int productID { get; set; }
        public string productName { get; set; }
        public string qty { get; set; }
        public string price { get; set; }
        public int branchid { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
    }
}