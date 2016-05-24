using System.Data.Entity;
using Test.Core.Domain;
using Test.Core.Interfaces;

namespace Test.Data
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext() : base("name=Test")
        {
            Database.SetInitializer<DataContext>(null);
        }

        public DbSet<Exercise> Tests { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }
    }
}