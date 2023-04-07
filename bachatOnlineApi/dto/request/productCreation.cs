using System;
using System.Text.Json.Serialization;


namespace bachatOnlineModuleApi.Entities
{
    public class ProductCreation
    {
        public string id { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string inventory { get; set; }
        // public string picEDoc { get; set; }
        // public string picEDocPath { get; set; }
        public int branchid { get; set; }
        public int businessid { get; set; }
        public int companyid { get; set; }
        // public string picEdocExtenstion { get; set; }
    }
}