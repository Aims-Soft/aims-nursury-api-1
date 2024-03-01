using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public class ClosingSaleDetail
    {
        public int shiftID { get; set; }
        public int invoiceNo { get; set; }
        public int productID { get; set; }
        public string productName { get; set; }
        public string barcode1 { get; set; }
        public int qty { get; set; }
        public float salePrice { get; set; }
        public float totalPrice { get; set; }
    }
}