using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRA.Scan.API.Entities.Entities
{
    public partial class UserRole : IdentityUserRole<long>
    {
        [Column("user_id")]
        public override long UserId { get; set; }
        [Column("role_id")]
        public override long RoleId { get; set; }
    }
}
