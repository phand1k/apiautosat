using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Models
{
    public class NotifiactionToken
    {
        public Guid Id { get; set; }
        public string? Token { get; set; }
        public DateTime? DateOfCreated { get; set; } = DateTime.UtcNow;
        public bool? IsDeleted { get; set; } = false;
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
    }
}
