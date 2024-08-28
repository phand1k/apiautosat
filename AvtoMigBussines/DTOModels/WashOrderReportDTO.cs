namespace AvtoMigBussines.DTOModels
{
    public class WashOrderReportDTO
    {
        public int? WashOrderId { get; set; }
        public string? CarNumber { get; set; }
        public string? Car { get; set; }
        public DateTime? DateOfCreatedWashOrder { get; set; }
        public DateTime? DateOfCompletedWashOrder { get; set; }
        public double? SummOfServicesOnOrder { get; set; }
        public int? CountOfServicesOnOrder { get; set; }
    }
}
