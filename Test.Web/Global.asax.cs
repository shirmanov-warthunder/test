using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Common.Logging;
using log4net.Config;
using Test.Web.Infrastructure.Plumbing;
using Test.Web.Models;

namespace Test.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer container;

        protected void Application_End()
        {
            container.Dispose();
        }

        private static void BootstrapContainer()
        {
            container = new WindsorContainer().Install(FromAssembly.This());

            DependencyResolver.SetResolver(new WindsorDependencyResolver(container));
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            XmlConfigurator.Configure();

            BootstrapContainer();
        }

        protected void Application_Error()
        {
            var logger = LogManager.GetLogger(typeof(MvcApplication));

            logger.Error(Server.GetLastError());
        }

        protected void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                string encTicket = authCookie.Value;
                if (!string.IsNullOrEmpty(encTicket))
                {
                    var ticket = FormsAuthentication.Decrypt(encTicket);
                    var id = new UserIdentity(ticket);
                    var roles = Roles.GetRolesForUser(id.Name);
                    var prin = new GenericPrincipal(id, roles);
                    HttpContext.Current.User = prin;
                }
            }
        }
    }
}