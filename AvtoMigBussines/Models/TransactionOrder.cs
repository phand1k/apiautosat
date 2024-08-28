using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Models
{
    public class TransactionOrder
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
        [ForeignKey("OrderId")]
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
        public TransactionOrder()
        {
            var timeZone = DateTimeZoneProviders.Tzdb["Asia/Almaty"];
            var now = SystemClock.Instance.GetCurrentInstant();
            DateOfCreated = now.InZone(timeZone).ToDateTimeUnspecified();
        }
    }
}
