using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Models
{
    public abstract class Order
    {
        public int Id { get; set; }
        public DateTime? DateOfCreated { get; set; } = DateTime.Now;
        public int? CarId { get; set; }
        public Car? Car { get; set; }
        public int? ModelCarId { get; set; }
        public ModelCar? ModelCar { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public bool? IsReturn { get; set; } = false;
        public bool? IsOvered { get; set; } = false;
        public DateTime? DateOfCompleteService { get; set; } = DateTime.Now;
        public string? PhoneNumber { get; set; }
        public Order()
        {
            var timeZone = DateTimeZoneProviders.Tzdb["Asia/Almaty"];
            var now = SystemClock.Instance.GetCurrentInstant();
            DateOfCreated = now.InZone(timeZone).ToDateTimeUnspecified();
            DateOfCompleteService = now.InZone(timeZone).ToDateTimeUnspecified();
        }

    }
}
