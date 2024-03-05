using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace reportApi.Entities
{
    public class DailyProductSale
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public string productNameUrdu { get; set; }
        public int productQty { get; set; }
        public int branchID { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public string invoiceDate { get; set; }
    }
}