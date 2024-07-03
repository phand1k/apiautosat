using AvtoMigBussines.Authenticate;
using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? IsDeleted { get; set; } = false;
    }
}
