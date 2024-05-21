using System.ComponentModel.DataAnnotations;
namespace AvtoMigBussines.Authenticate.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Organization Number is required")]
        public string? OrganizationId { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        public string? PhoneNumber { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SurName { get; set; }
        public string? IndividualNumberUser { get; set; }
    }
}
