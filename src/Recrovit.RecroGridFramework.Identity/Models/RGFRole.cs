using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Recrovit.RecroGridFramework.Identity.Models
{
    [Table("RGF_IdentityRole")]
    public class RGFRole
    {
        public RGFRole()
        {
            this.UserRole = new HashSet<RGFUserRole>();
        }

        [Key]
        [StringLength(255)]
        public string RoleId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string RoleName { get; set; }

        [StringLength(255)]
        public string RoleScope { get; set; }

        [StringLength(16)]
        public string Source { get; set; }

        [InverseProperty(nameof(RGFUserRole.Role))]
        public virtual ICollection<RGFUserRole> UserRole { get; set; }
    }
}
