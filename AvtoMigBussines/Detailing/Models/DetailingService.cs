﻿using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Models;
using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Detailing.Models
{
    public class DetailingService
    {
        public int Id { get; set; }
        public double? Price { get; set; }
        [ForeignKey("ServiceId")]
        public int? ServiceId { get; set; }
        public Service? Service { get; set; }
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public AspNetUser? AspNetUser { get; set; }
        public DateTime? DateOfCreated { get; set; } = DateTime.UtcNow;
        public bool? IsDeleted { get; set; } = false;
        public bool? IsOvered { get; set; } = false;
        [ForeignKey("DetailingOrderId")]
        public int? DetailingOrderId { get; set; }
        public DetailingOrder? DetailingOrder { get; set; }
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        [ForeignKey("WhomAspNetUserId")]
        public string? WhomAspNetUserId { get; set; }
        public double? Salary { get; set; }
        public DateTime? DateOfCompleteService { get; set; } = DateTime.UtcNow;
        public string? Comment { get; set; }

        public string ActionType => "DetailingService";
        public DetailingService()
        {
            var timeZone = DateTimeZoneProviders.Tzdb["Asia/Almaty"];
            var now = SystemClock.Instance.GetCurrentInstant();
            DateOfCreated = now.InZone(timeZone).ToDateTimeUnspecified();
        }
    }
}
