namespace UMISModuleAPI.Entities
{
    public class VerifyPin
    {
        public int newUserId { get; set; }
        public string pinCode { get; set; }
        public string spType { get; set; }
    }
}