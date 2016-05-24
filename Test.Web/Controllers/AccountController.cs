using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using AutoMapper;
using Common.Logging;
using Test.BL.Providers;
using Test.Core.Domain;
using Test.Core.Interfaces;
using Test.Resources;
using Test.Web.Models;
using Test.Web.Models.Account;

namespace Test.Web.Controllers
{
    [Ban]
    public class AccountController : Controller
    {
        private RegistrationProvider registrationProvider;

        public AccountController(IRepository<User> userRepository, IRepository<Role> roleRepository)
        {
            this.registrationProvider = new RegistrationProvider(userRepository, roleRepository);
            Mapper.CreateMap<User, RegistrationModel>();
            Mapper.CreateMap<RegistrationModel, User>();
            Mapper.CreateMap<User, ManageModel>();
            Mapper.CreateMap<ManageModel, User>();
        }

        #region Registre new user
        public ActionResult Registration(bool isAdmin = false)
        {
            ViewBag.IsAdmin = isAdmin;

            var model = new RegistrationModel()
            {
                Roles = Roles.GetAllRoles()
            };
            return this.View(model);
        }

        [HttpPost]
        public ActionResult Registration(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User();
                Mapper.Map(model, user);

                if (model.Role == "-1")
                {
                    return new HttpStatusCodeResult(500, "You must chose a role");
                }

                if (!string.IsNullOrEmpty(model.Role))
                {
                    user.IsEmailConfirmed = true;
                    this.registrationProvider.CreateUser(user);
                    Roles.AddUserToRole(model.Login, model.Role);
                    return this.RedirectToAction("Index", "Admin");
                }

                this.registrationProvider.CreateUser(user);
                this.registrationProvider.SendEmailReg(
                    user,
                    Url.Action("ConfirmEmail", "Account", new RouteValueDictionary(new { login = user.Login, email = user.Email }), Request.Url.Scheme));

                HttpContext.Cache[user.Login] = DateTime.Now.AddMinutes(1);

                return this.RedirectToAction("EmailWasSent");
            }

            ViewBag.IsAdmin = !string.IsNullOrEmpty(model.Role); // if true it is not the admin
            return this.View(model);
        }

        public ActionResult RegisterSuccess()
        {
            return this.View();
        }
        #endregion

        #region Login
        public ActionResult LogIn()
        {
            var model = new LogOnModel();
            return this.View(model);
        }

        [HttpPost]
        public ActionResult LogIn(LogOnModel model)
        {
            if (ModelState.IsValid)
            {
                if (this.registrationProvider.CheckUser(model.Login, model.Password))
                {
                    var logger = LogManager.GetLogger(typeof(AccountController));
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName,
                        this.registrationProvider.GetAuthenticationTicket(model.Login)));

                    logger.InfoFormat("User {0} log in the application", model.Login);
                    return this.RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("Login", AccountResources.ValidationLoginError);
            }

            return this.View(model);
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }

            return this.RedirectToAction("LogIn");
        }
        #endregion

        #region Manage

        public ActionResult ManageAccount(string login)
        {
            if (User.IsInRole("Administrator") || User.Identity.Name == login)
            {
                if (HttpContext.Cache[User.Identity.Name] != null)
                {
                    if (!this.registrationProvider.GetUser(User.Identity.Name).IsEmailConfirmed && (DateTime)HttpContext.Cache[User.Identity.Name] < DateTime.Now)
                    {
                        FormsAuthentication.SignOut();
                        this.registrationProvider.DeleteUser(User.Identity.Name);
                        return this.RedirectToAction("LogIn");
                    }
                }

                ViewBag.CanChangePassword = User.Identity.Name == login ? true : false;

                var user = this.registrationProvider.GetUser(login);

                var model = new ManageModel();

                Mapper.Map(user, model);

                return this.View(model);
            }

            return new HttpUnauthorizedResult("It's not yours account");
        }

        [HttpPost]
        public ActionResult ManageAccount(ManageModel model)
        {
            if (ModelState.IsValid)
            {
                var user = this.registrationProvider.GetUser(model.Login);

                if (string.IsNullOrEmpty(model.Password))
                {
                    var password = user.Password;
                    Mapper.Map(model, user);
                    user.Password = password;
                    this.registrationProvider.UpdateUser(user, false);
                }
                else
                {
                    Mapper.Map(model, user);
                    this.registrationProvider.UpdateUser(user, true);
                }

                return this.RedirectToAction("Index", "Home");
            }

            return this.View(model);
        }

        #endregion

        public ActionResult RestourePassword()
        {
            return this.View();
        }

        public ActionResult ConfirmEmail(string login, string email)
        {
            this.registrationProvider.ConfirmEmail(login, email);
            return this.RedirectToAction("Index", "Home");
        }

        public ActionResult ForgotPassword()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            this.registrationProvider.RestorePassword(email);
            return this.RedirectToAction("RestourePassword");
        }

        [AllowAnonymous]
        public ActionResult Ban()
        {
            return this.View();
        }

        public ActionResult EmailWasSent()
        {
            return this.View();
        }
    }
}