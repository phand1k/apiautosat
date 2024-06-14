using AvtoMigBussines.Authenticate;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
        public bool? IsDeleted { get; set; } = false;
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public AspNetUser? AspNetUser { get; set; }
    }
}
