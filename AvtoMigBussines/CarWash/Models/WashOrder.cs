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

        [ForeignKey("EndOfOrderAspNetUserId")]
        public string? EndOfOrderAspNetUserId { get; set; }
        public AspNetUser? EndOfOrderAspNetUser { get; set; }

        // Дополнительные свойства для хранения ФИО
        [NotMapped]
        public string? CreatedByFullName => AspNetUser != null ? $"{AspNetUser.FirstName} {AspNetUser.LastName} {AspNetUser.PhoneNumber}" : null;

        [NotMapped]
        public string? EndedByFullName => EndOfOrderAspNetUser != null ? $"{EndOfOrderAspNetUser.FirstName} {EndOfOrderAspNetUser.LastName} {EndOfOrderAspNetUser.PhoneNumber}" : null;
    }

}
