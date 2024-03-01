using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class ClosingCreation
    {
        public int shiftID { get; set; }
        public int moduleId { get; set; }
        public string shiftDate { get; set; }
        public string shiftEndTime { get; set; }
        public int closingBalance { get; set; }
        public int counterID { get; set; }
        public int counterUserID { get; set; }
        public int userID { get; set; }
        public string json { get; set; }
    }
}