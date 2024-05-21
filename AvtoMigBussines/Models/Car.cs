using System.ComponentModel.DataAnnotations;

namespace AvtoMigBussines.Models
{
    public class Car
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Car name is required field")]
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
