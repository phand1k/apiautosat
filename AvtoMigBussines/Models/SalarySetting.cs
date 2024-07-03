using AvtoMigBussines.Authenticate;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Models
{
    public class SalarySetting
    {
        public int Id { get; set; }
        [ForeignKey("ServiceId")]
        public int? ServiceId { get; set; }
        public Service? Service { get; set; }
        public double? Salary { get; set; }
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public AspNetUser? AspNetUser { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DateOfCreated { get; set; } = DateTime.Now;
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }
}
