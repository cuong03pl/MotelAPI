using System.ComponentModel.DataAnnotations;

namespace Motel.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
} 