using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Test.Core.Domain;

namespace Test.Web.Models.Home
{
    public class TestDetailsModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<TestResult> Results { get; set; }

        public IEnumerable<User> Participants { get; set; }
    }
}