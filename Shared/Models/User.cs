using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models.Identify
{
    [Serializable]
    [Table("Users")]
    public class User : Entity, IEntity
    {
        public AccountType Type { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    }

    public enum AccountType
    {
        SupperAdmin = 0,
        Admin = 1,
        Customer = 2
    }
}
