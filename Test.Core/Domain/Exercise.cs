using System.Collections.Generic;
using Test.Core.Domain.Base;

namespace Test.Core.Domain
{
    public class Exercise : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public virtual User Creator { get; set; }

        public virtual ICollection<TestResult> Participants { get; set; }
    }
}