using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Recrovit.RecroGridFramework.Identity.Models
{
    [Table("RGF_IdentityUser")]
    public class RGFUser
    {
        public RGFUser()
        {
            this.UserRole = new HashSet<RGFUserRole>();
        }
        
        [Key]
        [StringLength(255)]
        public string UserId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        [StringLength(3)]
        public string Language { get; set; }

        [InverseProperty(nameof(RGFUserRole.User))]
        public virtual ICollection<RGFUserRole> UserRole { get; set; }
    }
}
