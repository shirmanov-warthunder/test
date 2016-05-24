using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Test.Core.Domain;
using Test.Resources;

namespace Test.Web.Models.Account
{
    public class ManageModel
    {
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(AccountResources), Name = "NewPassword")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(AccountResources), ErrorMessageResourceName = "Error_BadConfirmPassword")]
        [Display(ResourceType = typeof(AccountResources), Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [Display(ResourceType = typeof(AccountResources), Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(AccountResources), Name = "LastName")]
        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<TestResult> PassedTests { get; set; }
    }
}