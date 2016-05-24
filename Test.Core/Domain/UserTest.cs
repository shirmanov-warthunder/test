using System;
using Test.Core.Domain.Base;
using Test.Core.Domain.Enum;

namespace Test.Core.Domain
{
    public class TestResult : BaseEntity
    {
        public int UserId { get; set; }

        public int ExerciseId { get; set; }

        public Grade? Grade { get; set; }

        public DateTime TestDate { get; set; }

        public virtual User User { get; set; }

        public virtual Exercise Exercise { get; set; }
    }
}