using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DentistShop
{
    public class ShopUserViewModel
    {

        [Key]
        public int userId { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string userName { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string firstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string lastName { get; set; }

        [Display(Name = "آدرس ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        public string emailAddress { get; set; }

        [Display(Name = "رمز عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = " تکرار رمز")]
        [Required(ErrorMessage = "لطفا  {0} را کامل کنید")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "کلمه های عبور مغایرت دارند!")]
        public string RePassword { get; set; }

        //public string pic { get; set; }
        [Display(Name = "تلفن همراه")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string phone { get; set; }

        [Display(Name = "آدرس")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string Address { get; set; }
    }
}