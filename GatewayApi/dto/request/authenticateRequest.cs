using System.ComponentModel.DataAnnotations;

namespace gatewayapi.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string Loginname { get; set; }

        [Required]
        public string hashpassword { get; set; }
    }
}