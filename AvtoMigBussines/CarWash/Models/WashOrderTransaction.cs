using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;
using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.CarWash.Models
{
    public class WashOrderTransaction
    {
        public int Id { get; set; }
        [ForeignKey("PaymentMethodId")]
        public int? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public DateTime? DateOfCreated { get; set; } = DateTime.Now;
        public double? Summ { get; set; }
        public bool? IsDeleted { get; set; } = false;
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        [ForeignKey("WashOrderId")]
        public int? WashOrderId { get; set; }
        public WashOrder? WashOrder { get; set; }
        public WashOrderTransaction()
        {
            var timeZone = DateTimeZoneProviders.Tzdb["Asia/Almaty"];
            var now = SystemClock.Instance.GetCurrentInstant();
            DateOfCreated = now.InZone(timeZone).ToDateTimeUnspecified();
        }
    }
}
