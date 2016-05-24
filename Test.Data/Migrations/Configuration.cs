using System.Collections.ObjectModel;
using System.Linq;
using Test.Core.Domain;
using Test.Core.Domain.Enum;

namespace Test.Data.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Test.Data.DataContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DataContext context)
        {
            //context.Roles.AddOrUpdate(m => m.Name,
            //    new Role() { Name = RolesName.RegisteredUser },
            //    new Role() { Name = RolesName.Student },
            //    new Role() { Name = RolesName.AdvancedStudent },
            //    new Role() { Name = RolesName.Tutor },
            //    new Role() { Name = RolesName.AdvancedTutor },
            //    new Role() { Name = RolesName.Administrator },
            //    new Role() { Name = RolesName.Disabled });

            var creator = new User()
            {
                Login = "creator",
                Email = "creator@god.com",
                Password = "5fa285e1bebe0a6623e33afc04a1fbd5",
                Roles = new Collection<Role>() { new Role() { Name = RolesName.Tutor } }
            };

            context.Users.AddOrUpdate(m => m.Login,
                new User()
                {
                    Login = "admin",
                    Email = "admin@company.com",
                    Password = "35106240d2f0db35716e127eb80a0299",
                    IsEmailConfirmed = true,
                    Roles = new Collection<Role>()
                    {
                        new Role() { Name = RolesName.Administrator }
                    }
                });

            for (int i = 0; i < 1000; i++)
            {
                context.Tests.AddOrUpdate(m => m.Name,
                    new Exercise()
                    {
                        Name = i.ToString(),
                        Description = i.ToString(),
                        Creator = creator
                    });
            }
        }
    }
}