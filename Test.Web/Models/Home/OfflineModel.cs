using System.Collections.Generic;
using Test.Core.Domain;

namespace Test.Web.Models.Home
{
    public class OfflineModel
    {
        public IEnumerable<User> Students { get; set; }

        public IEnumerable<Exercise> Tests { get; set; }
    }
}