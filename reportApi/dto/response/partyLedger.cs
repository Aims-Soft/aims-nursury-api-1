using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class PartyLedger
    {
        public int coaid { get; set; }
        public string coatitle { get; set; }
        public int partyid { get; set; }
        public string partyname { get; set; }
        public string type { get; set; }
        public int invoiceno { get; set; }
        public string invoicetype { get; set; }
        public string invoicedate { get; set; }
        public int refinvoiceno { get; set; }
        public string instrumentno { get; set; }
        public string instrumentdate { get; set; }
        public string description { get; set; }
        public int branchID { get; set; }
        public float debit { get; set; }
        public float credit { get; set; }
    }
}