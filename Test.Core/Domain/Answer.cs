using Test.Core.Domain.Base;

namespace Test.Core.Domain
{
    public class Answer : BaseEntity
    {
        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }

        public string Text { get; set; }

        public bool IsRight { get; set; }
    }
}