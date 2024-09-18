using System.ComponentModel.DataAnnotations;

namespace AvtoMigBussines.Models
{
    public class ForgotPasswordCode
    {
        public int Id { get; set; }
        [StringLength(4)]
        public double? Code { get; set; }
        public DateTime? DateOfCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateOfEnd { get; set; }
        [StringLength(11)]
        public string? PhoneNumber { get; set; }
    }
}
