using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class OpeningCreation
    {
        public int shiftID { get; set; }
        public int moduleId { get; set; }
        public string shiftDate { get; set; }
        public string shiftStartTime { get; set; }
        public int openingBalance { get; set; }
        public int counterID { get; set; }
        public int userID { get; set; }
        public string json { get; set; }
    }
}