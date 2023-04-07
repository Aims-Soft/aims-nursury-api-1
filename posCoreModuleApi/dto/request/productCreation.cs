using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class ProductCreation
    {
        public int productID { get; set; }
        public Nullable<int> locationID { get; set; }
        public int brandID { get; set; }
        public int categoryID { get; set; }
        public int uomID { get; set; }
        public Nullable<int> barcodeID { get; set; }
        public Nullable<int> pPriceID { get; set; }
        public string productName { get; set; }
        public string productNameUrdu { get; set; }
        public string barcode1 { get; set; }
        public string barcode2 { get; set; }
        public string barcode3 { get; set; }
        public float costPrice { get; set; }
        public float salePrice { get; set; }
        public float retailPrice { get; set; }
        public float wholeSalePrice { get; set; }
        public Nullable<int> reOrderLevel { get; set; }
        public Nullable<int> maxLimit { get; set; }
        public Nullable<float> gst { get; set; }
        public Nullable<float> et { get; set; }
        public Nullable<int> packingQty { get; set; }
        public Nullable<float> packingSalePrice { get; set; }
        public string pctCode { get; set; }
        public string quickSale { get; set; }
        public string applicationEDocPath { get; set; }
        public string applicationEDoc { get; set; }
        public string applicationEdocExtenstion { get; set; }
        public int userID { get; set; }
        public Nullable<int> colorID { get; set; }
        public Nullable<int> sizeID { get; set; }
        public int branchid { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        public string potType { get; set; }
        public string potSize { get; set; }
        public string mfgDate { get; set; }
        public string expDate { get; set; }
    }
}