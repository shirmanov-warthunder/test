using System.Security.Principal;
using System.Web.Security;

namespace Test.Web.Models
{
    public class UserIdentity : IIdentity, IPrincipal
    {
        private readonly FormsAuthenticationTicket ticket;

        public UserIdentity(FormsAuthenticationTicket ticket)
        {
            this.ticket = ticket;
        }

        public string AuthenticationType
        {
            get { return "User"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return this.ticket.Name; }
        }

        public string UserId
        {
            get { return this.ticket.UserData; }
        }

        public bool IsInRole(string role)
        {
            return Roles.IsUserInRole(role);
        }

        public IIdentity Identity
        {
            get { return this; }
        }
    }
}