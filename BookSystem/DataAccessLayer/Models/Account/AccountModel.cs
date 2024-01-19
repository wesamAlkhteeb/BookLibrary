using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Account
{
    public class AccountModel
    {
        [Required(ErrorMessage ="Name is required")]
        [MinLength(4 ,ErrorMessage ="length must be at least 4 letters.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(pattern: "^(?=.*[a-z])(?=.*[A-Z])(?=.*[\\W_])(?=.*\\d)[a-zA-Z\\d\\W_]{8,}$", ErrorMessage = "Password is invalid. You must to input at lest 8 character consists of capital and small character, digit and symbols.")]
        public required string Password { get; set; }
    }
}
