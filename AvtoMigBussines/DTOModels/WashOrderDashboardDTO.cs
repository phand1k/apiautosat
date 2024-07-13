namespace AvtoMigBussines.DTOModels
{
    public class WashOrderDashboardDTO
    {
        public int? CountOfNotCompletedOrders { get; set; }
        public int? CountOfNotCompletedServices { get; set; }
        public int? CountOfCompeltedServices { get; set; }
        public double? SummOfAllServices { get; set; }
    }
}
