using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Test.Core.Domain;
using Test.Core.Domain.Enum;
using Test.Core.Interfaces;
using Test.Web.Models.Admin;

namespace Test.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private IRepository<User> userRepository;
        private IRepository<Role> roleRepository;

        public AdminController(IRepository<User> userRepository, IRepository<Role> roleRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        public ActionResult Index()
        {
            var model = this.userRepository.GetAll();
            return this.View(model);
        }

        public ActionResult AddUserToRole()
        {
            var model = new UserRoleModel()
            {
                Users = this.userRepository.GetAll(),
                Roles = this.roleRepository.GetAll()
            };
            return this.View(model);
        }

        [HttpPost]
        public ActionResult AddUserToRole(UserRoleModel model)
        {
            if (model.Login != null)
            {
                if (model.Login != "-1" && model.Role.ToString() != "-1")
                {
                    if (!model.ToDelete)
                    {
                        Roles.AddUserToRole(model.Login, model.Role.ToString());
                    }
                    else
                    {
                        Roles.RemoveUserFromRole(model.Login, model.Role.ToString());
                    }
                }
            }

            model = new UserRoleModel()
            {
                Users = this.userRepository.GetAll(),
                Roles = this.roleRepository.GetAll()
            };

            return this.View(model);
        }

        public ActionResult Ban()
        {
            var model = new UserRoleModel()
            {
                Users = this.userRepository.GetAll()
            };

            return this.View(model);
        }

        [HttpPost]
        public ActionResult Ban(UserRoleModel model)
        {
            var roleName = model.Role.ToString();
            if (model.Role == RolesName.Disabled)
            {
                Roles.AddUserToRole(model.Login, roleName);
            }
            else
            {
                Roles.RemoveUserFromRole(model.Login, RolesName.Disabled.ToString());
            }

            model.Users = this.userRepository.GetAll();

            return this.View(model);
        }

        [HttpPost]
        public JsonResult IsUserActive(string id)
        {
            return Roles.IsUserInRole(id, "Disabled") ? this.Json(new { State = "Disabled" }) : this.Json(new { State = "Active" });
        }
    }
}