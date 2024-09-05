using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Models;
using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Detailing.Models
{
    public class DetailingOrderTransaction
    {
        public int Id { get; set; }
        [ForeignKey("PaymentMethodId")]
        public int? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public AspNetUser? AspNetUser { get; set; }
        public DateTime? DateOfCreated { get; set; } = DateTime.UtcNow;
        public double? Summ { get; set; }
        public double? ToPay { get; set; }
        public bool? IsDeleted { get; set; } = false;
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        [ForeignKey("DetailingOrderId")]
        public int? DetailingOrderId { get; set; }
        public DetailingOrder? DetailingOrder { get; set; }
        public DetailingOrderTransaction()
        {
            var timeZone = DateTimeZoneProviders.Tzdb["Asia/Almaty"];
            var now = SystemClock.Instance.GetCurrentInstant();
            DateOfCreated = now.InZone(timeZone).ToDateTimeUnspecified();
        }
    }
}
