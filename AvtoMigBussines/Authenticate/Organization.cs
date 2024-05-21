using AvtoMigBussines.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AvtoMigBussines.Authenticate
{
    public class Organization
    {
        public int Id { get; set; }
        [Required]
        [StringLength(12, ErrorMessage = " Поле должно содержать в себе 12 символов", MinimumLength = 12)]
        public string? Number { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = " Поле должно содержать в себе 12 символов", MinimumLength = 2)]
        public string? Name { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = " Поле должно содержать в себе 12 символов", MinimumLength = 2)]
        public string? FullName { get; set; }
        public DateTime? DateOfCreated { get; set; } = DateTime.Now;
        public bool? IsDeleted { get; set; } = false;
        [JsonIgnore]
        public ICollection<AspNetUser>? AspNetUsers { get; set; }
        public Organization()
        {
            AspNetUsers = new List<AspNetUser>();
        }
    }
}
