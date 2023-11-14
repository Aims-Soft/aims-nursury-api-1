using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public class ProductImportCreation
    {
        public string product_category { get; set; }
        public string product_sub_category { get; set; }
        public string product_name { get; set; }
        public float cost_price { get; set; }
        public float sale_price { get; set; }
        public string product_barcode { get; set; }
    }
}