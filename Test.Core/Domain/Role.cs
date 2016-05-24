using System.Collections.Generic;
using Test.Core.Domain.Base;
using Test.Core.Domain.Enum;

namespace Test.Core.Domain
{
    public class Role : BaseEntity
    {
        public RolesName Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}