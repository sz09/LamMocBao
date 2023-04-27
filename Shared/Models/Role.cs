using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models.Identify
{
    [Serializable]
    [Table("Roles")]
    public class Role : Entity, IEntity
    {
        public string Type { get; set; }
        public string NormalizedRole { get; set; }
        public string Claim { get; set; }
        public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    }

    [Serializable]
    [Table("UserRoles")]
    public class UserRole : Entity, IEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
