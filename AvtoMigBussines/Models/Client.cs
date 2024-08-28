using System.ComponentModel.DataAnnotations;

namespace AvtoMigBussines.Models
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        public long? TelegramUserId { get; set; }
        [Required]
        public string? CarNumber { get; set; }
        public int? OrganizationId { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DateOfRegister { get; set; } = DateTime.UtcNow;
    }
}
