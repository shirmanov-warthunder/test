using System.Collections.Generic;
using Test.Core.Domain.Base;

namespace Test.Core.Domain
{
    public class Question : BaseEntity
    {
        public int TestId { get; set; }

        public virtual Exercise Test { get; set; }

        public string Text { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}