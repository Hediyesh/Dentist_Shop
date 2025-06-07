using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DentistShop
{
    public class RecoveryPasswordViewModel
    {
        [Display(Name = "رمز عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = "تکرار رمز جدید")]
        [Required(ErrorMessage = "لطفا  {0} را کامل کنید")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "کلمه های عبور مغایرت دارند!")]
        public string RePassword { get; set; }
    }
}