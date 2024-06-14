using AvtoMigBussines.Authenticate;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public DateTime? DateOfCreated { get; set; } = DateTime.Now;
        public DateTime? DateOfStart { get; set; } = DateTime.Now;
        public DateTime? DateOfEnd { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public bool? IsReturn { get; set; } = false;
    }
}
