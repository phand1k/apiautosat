namespace AvtoMigBussines.DTOModels
{
    public class SalarySettingDTO
    {
        public int ServiceId { get; set; }
        public string? AspNetUserId { get; set; }
        public int? OrganizationId { get; set; }
        public double? Salary { get; set; }
    }
}
