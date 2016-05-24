using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Test.BL.Providers;
using Test.Core.Domain;
using Test.Core.Interfaces;
using Test.Resources;

namespace Test.Web.Models.Account
{
    public class RegistrationModel : IValidatableObject
    {
        public string[] Roles { get; set; }

        public string Role { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountResources), ErrorMessageResourceName = "Error_EmptyTextField")]
        public string Login { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountResources), ErrorMessageResourceName = "Error_EmptyTextField")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountResources), ErrorMessageResourceName = "Error_EmptyTextField")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(AccountResources), Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountResources), ErrorMessageResourceName = "Error_EmptyTextField")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(AccountResources), ErrorMessageResourceName = "Error_BadConfirmPassword")]
        [Display(ResourceType = typeof(AccountResources),  Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [Display(ResourceType = typeof(AccountResources), Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(AccountResources), Name = "LastName")]
        public string LastName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var registerProvider = new RegistrationProvider(DependencyResolver.Current.GetService<IRepository<User>>(), DependencyResolver.Current.GetService<IRepository<Role>>());

            if (!registerProvider.IsLoginFree(this.Login))
            {
                yield return new ValidationResult(AccountResources.Error_LoginAlreadyExists, new[] { "Login" });
            }

            if (!registerProvider.IsEmailFree(this.Email))
            {
                yield return new ValidationResult(AccountResources.Error_EmailAlreadyExists, new[] { "Email" });
            }
        }
    }
}