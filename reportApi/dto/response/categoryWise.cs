using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class CategoryWise
    {
        public string invoicedate { get; set; }
        public int categoryid { get; set; }
        public string categoryname { get; set; }
        public float totalqty { get; set; }
        public float totalsaleprice { get; set; }
        public float totalcostprice { get; set; }
        public float marginamount { get; set; }
        public string invoicetype { get; set; }
    }
}