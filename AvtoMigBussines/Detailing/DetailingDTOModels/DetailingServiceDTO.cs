namespace AvtoMigBussines.Detailing.DetailingDTOModels
{
    public class DetailingServiceDTO
    {
        public int DetailingServiceId { get; set; }
        public int? ServiceId { get; set; }
        public int? DetailingOrderId { get; set; }
        public double? Price { get; set; }
        public string? ServiceName { get; set; }
        public string? WhomAspNetUserId { get; set; }
        public string? Order { get; set; }
        public string? CarNumber { get; set; }
        public string? AspNetUserId { get; set; }
        public double? Salary { get; set; }
        public DateTime? DateOfCreated { get; set; }
        public DateTime? DateOfCompleted { get; set; }
        public bool? IsOvered { get; set; }
        public string? CreatedAspNetUserId { get; set; }
    }
}
