using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.CarWash.Models
{
    public class WashOrder : Order
    {
        [Required]
        public string? CarNumber { get; set; }
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public AspNetUser? AspNetUser { get; set; }
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }
}
