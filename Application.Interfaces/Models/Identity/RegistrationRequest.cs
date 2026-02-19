using System.ComponentModel.DataAnnotations;

namespace Application.Interfaces.Models.Identity
{
    public class RegistrationRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }

    }
}
