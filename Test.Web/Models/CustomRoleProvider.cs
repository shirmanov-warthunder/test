using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Test.Core.Domain;
using Test.Core.Domain.Enum;
using Test.Core.Interfaces;

namespace Test.Web.Models
{
    public class CustomRoleProvider : RoleProvider
    {
        public IRepository<User> UserRepository
        {
            get
            {
                return DependencyResolver.Current.GetService<IRepository<User>>();
            }
        }

        public IRepository<Role> RoleRepository
        {
            get
            {
                return DependencyResolver.Current.GetService<IRepository<Role>>();
            }
        }

        public override void CreateRole(string roleName)
        {
            this.RoleRepository.Create(new Role() { Name = (RolesName)Enum.Parse(typeof(RolesName), roleName) });
            this.RoleRepository.Save();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (throwOnPopulatedRole)
            {
                var role = this.RoleRepository.GetAll().SingleOrDefault(r => r.Name.ToString() == roleName);

                this.RoleRepository.Delete(role);

                this.RoleRepository.Save();
                return true;
            }

            this.RoleRepository.Delete(this.RoleRepository.GetAll().FirstOrDefault(r => r.Name.ToString() == roleName));
            this.RoleRepository.Save();
            return true;
        }

        public override string[] GetAllRoles()
        {
            return this.RoleRepository.GetAll().Select(r => r.Name.ToString()).ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            return this.UserRepository
                            .GetAll()
                            .First(u => u.Login == username)
                            .Roles
                            .Select(r => r.Name.ToString())
                            .ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var user = this.UserRepository.GetAll().SingleOrDefault(u => u.Login == username);
            if (user == null)
            {
                return false;
            }

            return user.Roles != null && user.Roles.Any(r => r.Name.ToString() == roleName);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (var username in usernames)
            {
                var user = this.UserRepository.GetAll().Single(m => m.Login == username);
                foreach (var roleName in roleNames)
                {
                    var role = this.RoleRepository.GetAll().First(r => r.Name.ToString() == roleName);
                    user.Roles.Add(role);
                }
            }

            this.UserRepository.Save();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            foreach (var username in usernames)
            {
                var user = this.UserRepository.GetAll().Single(m => m.Login == username);
                foreach (var roleName in roleNames)
                {
                    var role = this.RoleRepository.GetAll().First(r => r.Name.ToString() == roleName);
                    if (user.Roles.Any(r => r.Name == role.Name))
                    {
                        user.Roles.Remove(role);
                    }
                }
            }

            this.UserRepository.Save();
        }

        #region Not Implemented
        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}