using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Detailing.Models
{
    public class DetailingPriceList
    {
        public int Id { get; set; }
        public int? CarId { get; set; }
        public int? ModelCarId { get; set; }
        public double? Price { get; set; }
        public bool? IsDeleted { get; set; } = false;
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public AspNetUser? AspNetUser { get; set; }
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        [ForeignKey("Service")]
        public int? ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
