using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Recrovit.RecroGridFramework.Identity.Models
{
    [Table("RGF_IdentityUserRole")]
    public class RGFUserRole
    {
        [Key]
        [StringLength(255)]
        [Column(Order = 0)]
        public string UserId { get; set; }
        
        [Key]
        [StringLength(255)]
        [Column(Order = 1)]
        public string RoleId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(RGFUser.UserRole))]
        public virtual RGFUser User { get; set; }
        
        [ForeignKey(nameof(RoleId))]
        [InverseProperty(nameof(RGFUser.UserRole))]
        public virtual RGFRole Role { get; set; }
    }
}
