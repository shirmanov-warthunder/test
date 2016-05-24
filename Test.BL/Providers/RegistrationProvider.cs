using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using Test.Core.Domain;
using Test.Core.Interfaces;

namespace Test.BL.Providers
{
    public class RegistrationProvider
    {
        #region prop and ctors
        private IRepository<User> userRepository;
        private IRepository<Role> roleRepository;

        public RegistrationProvider(IRepository<User> userRepository, IRepository<Role> roleRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }
        #endregion

        #region HelpFunc
        public static string Hash(string password)
        {
            var passwordByte = Encoding.Unicode.GetBytes(password);
            var crypto = new MD5CryptoServiceProvider();

            var byteHash = crypto.ComputeHash(passwordByte);

            return byteHash.Aggregate(string.Empty, (current, b) => current + string.Format("{0:x2}", b));
        }

        public void SendEmailReg(User user, string urlPath)
        {
            var message = string.Format("Для завершения регистрации перейдите по ссылке:" +
                                        "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                urlPath);
            this.SendEmail(user, message);
        }

        public void SendEmail(User user, string message)
        {
            var from = new MailAddress("vladimir.shirmanov@gmail.com", "Web Registration");
            var to = new MailAddress(user.Email);
            var m = new MailMessage(from, to)
            {
                Subject = "Email confirmation",
                Body = message,
                IsBodyHtml = true
            };

            var smtp = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("vladimir.shirmanov@gmail.com", "tktyflelrj")
            };

            smtp.Send(m);
        }

        public string GetAuthenticationTicket(string login)
        {
            var user = this.userRepository.GetAll().SingleOrDefault(m => m.Login == login);

            if (user == null)
            {
                throw new Exception("No user with this login was found");
            }

            var userId = user.Id;
            var userData = userId.ToString(CultureInfo.InvariantCulture);
            var authTicket = new FormsAuthenticationTicket(1,
                login,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                userData);

            return FormsAuthentication.Encrypt(authTicket);
        }

        #endregion

        #region working with user
        public bool CheckUser(string login, string password)
        {
            return this.userRepository.GetAll().FirstOrDefault(u => u.Login.Equals(login.ToLower()) && u.Password.Equals(Hash(password))) != null;
        }

        public bool IsLoginFree(string login)
        {
            return this.userRepository.GetAll().FirstOrDefault(u => u.Login.Equals(login.ToLower())) == null;
        }

        public bool IsEmailFree(string email)
        {
            return this.userRepository.GetAll().FirstOrDefault(u => u.Email.Equals(email.ToLower())) == null;
        }

        public void CreateUser(User user)
        {
            user.Password = Hash(user.Password);

            user.Roles.Add(this.roleRepository.GetAll().FirstOrDefault(m => m.Name.ToString() == "RegisteredUser"));
            this.userRepository.Create(user);

            this.userRepository.Save();
        }

        public void ConfirmEmail(string login, string email)
        {
            var user = this.userRepository.GetAll().FirstOrDefault(u => u.Login == login);
            if (user.Email != email)
            {
                return;
            }

            user.IsEmailConfirmed = true;
            user.Roles.Add(this.roleRepository.GetAll().FirstOrDefault(m => m.Name.ToString() == "Student"));

            this.userRepository.Save();
        }

        public void RestorePassword(string email)
        {
            var user = this.userRepository.GetAll().FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                var newPassword = Membership.GeneratePassword(5, 0);

                user.Password = Hash(newPassword);

                var message =
                    string.Format(
                        "Вы отправили запрос на востановление пароля\nВаш логи {0}\nВаш временный пароль {1}\nПосле входа в систему рекомендуем сменить пароль",
                        user.Login,
                        newPassword);

                this.SendEmail(user, message);
                this.userRepository.Save();
            }
        }

        public User GetUser(string login)
        {
            var user = this.userRepository.GetAll().FirstOrDefault(m => m.Login == login);

            return user;
        }

        public void UpdateUser(User user, bool isNewPassword)
        {
            if (isNewPassword)
            {
                user.Password = Hash(user.Password);
            }

            this.userRepository.Update(user);
            this.userRepository.Save();
        }


        public void DeleteUser(string name)
        {
            var user = this.userRepository.GetAll().SingleOrDefault(m => m.Login == name);

            this.userRepository.Delete(user);

            this.userRepository.Save();
        }
        #endregion
    }
}
