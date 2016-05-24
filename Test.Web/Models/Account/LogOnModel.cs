using System.ComponentModel.DataAnnotations;

namespace Test.Web.Models.Account
{
    public class LogOnModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}