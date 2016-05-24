using System.ComponentModel.DataAnnotations;

namespace Test.Web.Models.Home
{
    public class TestModel
    {
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string CreatorLogin { get; set; }
    }
}