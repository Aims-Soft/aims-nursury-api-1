using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Product
    {
        public int productID { get; set; }
        public int locationID { get; set; }
        public int brandID { get; set; }
        public int categoryID { get; set; }
        public int uomID { get; set; }
        public int sizeID { get; set; }
        public int colorID { get; set; }
        public int barcodeID { get; set; }
        public int pPriceID { get; set; }
        public int parentCategoryID { get; set; }
        public int parentLocationID { get; set; }
        public string productName { get; set; }
        public string productNameUrdu { get; set; }
        public string barcode1 { get; set; }
        public string barcode2 { get; set; }
        public string barcode3 { get; set; }
        public float costPrice { get; set; }
        public float salePrice { get; set; }
        public float retailPrice { get; set; }
        public float wholeSalePrice { get; set; }
        public int ROL { get; set; }
        public int maxLimit { get; set; }
        public string quickSale { get; set; }
        public string pctCode { get; set; }
        public float gst { get; set; }
        public float et { get; set; }
        public int packing { get; set; }
        public float packingSalePrice { get; set; }
        public string categoryName { get; set; }
        public string level1 { get; set; }
        public string level2 { get; set; }
        public string level3 { get; set; }
        public string level4 { get; set; }
        public string level5 { get; set; }
        public string brandName { get; set; }
        public string uomName { get; set; }
        public string locationName { get; set; }
        public string applicationedoc { get; set; }
        public int branchid { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public string potType { get; set; }
        public string potSize { get; set; }
        public string mfgDate { get; set; }
        public string expDate { get; set; }
        public int qty { get; set; }
    }
}