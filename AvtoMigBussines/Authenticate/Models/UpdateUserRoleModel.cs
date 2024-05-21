using System.ComponentModel.DataAnnotations;

namespace AvtoMigBussines.Authenticate.Models
{
    public class UpdateUserRoleModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string RoleId { get; set; }
    }
}
