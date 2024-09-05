using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AvtoMigBussines.Detailing.Models
{
    public class DetailingOrder : Order
    {
        [Required]
        public string? CarNumber { get; set; }
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        [JsonIgnore]
        public AspNetUser? AspNetUser { get; set; }
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        [JsonIgnore]
        public Organization? Organization { get; set; }
        [ForeignKey("EndOfOrderAspNetUserId")]
        public string? EndOfOrderAspNetUserId { get; set; }
        public AspNetUser? EndOfOrderAspNetUser { get; set; }
        public string? Comment { get; set; }
        public double? Prepayment { get; set; }
        [NotMapped]
        public string? CreatedByFullName => AspNetUser != null ? $"{AspNetUser.FirstName} {AspNetUser.LastName} {AspNetUser.PhoneNumber}" : null;

        [NotMapped]
        public string? EndedByFullName => EndOfOrderAspNetUser != null ? $"{EndOfOrderAspNetUser.FirstName} {EndOfOrderAspNetUser.LastName} {EndOfOrderAspNetUser.PhoneNumber}" : null;

    }
}
