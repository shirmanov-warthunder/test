using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Test.Core.Domain.Base;

namespace Test.Core.Domain
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Login { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public virtual ICollection<Exercise> Tests { get; set; }

        public virtual ICollection<TestResult> PassedTests { get; set; }

        public virtual ICollection<Role> Roles { get; set; }

        public User()
        {
            this.Roles = new Collection<Role>();
            this.Tests = new Collection<Exercise>();
        }
    }
}