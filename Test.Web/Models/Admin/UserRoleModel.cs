using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Test.Core.Domain;
using Test.Core.Domain.Enum;

namespace Test.Web.Models.Admin
{
    public class UserRoleModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public RolesName Role { get; set; }

        public bool ToDelete { get; set; }

        public IEnumerable<User> Users { get; set; }

        public IEnumerable<Role> Roles { get; set; }
    }
}