using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Detailing.Models
{
    public class DetailingOrder : Order
    {
        [Required]
        public string? CarNumber { get; set; }
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public AspNetUser? AspNetUser { get; set; }
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        [ForeignKey("EndOfOrderAspNetUserId")]
        public string? EndOfOrderAspNetUserId { get; set; }
        public double? MileAge { get; set; }
        public string? ClientFullName { get; set; }
        public string? Comment { get; set; }

    }
}
