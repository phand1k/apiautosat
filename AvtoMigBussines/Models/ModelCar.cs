using AvtoMigBussines.Authenticate;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvtoMigBussines.Models
{
    public class ModelCar
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Model car name is required")]
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        [ForeignKey("CarId")]
        public int? CarId { get; set; }
        public Organization? Organization { get; set; }
    }
}
