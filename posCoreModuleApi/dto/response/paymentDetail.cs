using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class PaymentDetail
    {
        public int invoiceNo { get; set; }
        public int coaID { get; set; }
        public float debit { get; set; }
        public float credit { get; set; }
    }
}