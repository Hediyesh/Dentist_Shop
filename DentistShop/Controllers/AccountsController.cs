using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DataLayer;

namespace DentistShop.Controllers
{
    public class AccountsController : Controller
    {
        private DentistShopDBEntities db = new DentistShopDBEntities();
        // GET: Accounts
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(string namekarbar, string ramz)
        {
            string hashPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(ramz, "MD5");
            var user = db.ShopUser.Where(w => w.password == hashPassword && w.userName == namekarbar).SingleOrDefault();
            if (user != null)
            {
                if (user.IsActive == true)
                {
                    FormsAuthentication.SetAuthCookie(user.userName,true);
                    return Json(true);
                }
                else
                {
                    return Json("حساب کاربری شما فعال نمی باشد.");
                }
            }
            return Json("کاربری با این اطلاعات وجود ندارد.");
        }
        [Route("Register")]
        public ActionResult Register()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("Register")]
        public ActionResult Register(ShopUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (db.ShopUser.Any(a => a.userName == user.userName))
                {
                    ModelState.AddModelError("userName", "نام کاربری وارد شده تکراری می باشد.");
                    return View(user);
                }
                if (db.ShopUser.Any(a => a.emailAddress == user.emailAddress.Trim().ToLower()))
                {
                    ModelState.AddModelError("emailAddress", "ایمیل وارد شده تکراری می باشد.");
                    return View(user);
                }
                if (!db.ShopUser.Any(a => a.userName == user.userName) && !db.ShopUser.Any(a => a.emailAddress == user.emailAddress.Trim().ToLower()))
                {
                    int id = 1;
                    if (db.ShopUser.ToList().Count != 0)
                    {
                        id = db.ShopUser.ToList().Count + 1;
                    }
                    ShopUser shopuser = new ShopUser()
                    {
                        emailAddress = user.emailAddress.Trim().ToLower(),
                        password = FormsAuthentication.HashPasswordForStoringInConfigFile(user.password, "MD5"),
                        ActiveCode = Guid.NewGuid().ToString(),
                        IsActive = true,
                        RoleId = 1,
                        RegisterDate = DateTime.Now,
                        userName = user.userName,
                        firstName = user.firstName,
                        lastName = user.lastName,
                        phone = user.phone,
                        Address = user.Address,
                        pic = "",
                        userId = id
                    };
                    db.ShopUser.Add(shopuser);
                    db.SaveChanges();
                    return View("SuccessRegister", user);
                }
            }
            return View(user);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult CheckName(string username)
        {
            if (db.ShopUser.Any(a => a.userName == username))
            {
                return Json("نام کاربری وارد شده تکراری می باشد.");
            }
            return Json("");
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult CheckEmail(string email)
        {
            if (db.ShopUser.Any(a => a.emailAddress == email.Trim().ToLower()))
            {
                return Json("ایمیل وارد شده تکراری می باشد.");
            }
            return Json("");
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
        public ActionResult ActiveUser(string id)
        {
            var user = db.ShopUser.Where(w => w.ActiveCode == id).SingleOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            user.IsActive = true;
            user.ActiveCode = Guid.NewGuid().ToString();
            db.SaveChanges();
            ViewBag.name = user.userName;
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel forgot)
        {
            if (ModelState.IsValid)
            {
                var user = db.ShopUser.SingleOrDefault(s => s.emailAddress == forgot.emailAddress);
                if (user != null)
                {
                    if (user.IsActive)
                    {
                        string bodyEmail = PartialToStringClass.RenderPartialView("ManageEmails", "RecoveryPassword", user);
                        SendEmail.Send(user.emailAddress, "بازیابی رمز عبور", bodyEmail);
                        return View("SuccessForgotPassword", user);
                    }
                    else
                    {
                        ModelState.AddModelError("emailAddress", "حساب کاربری شما فعال نمی باشد.");
                    }
                }
                else
                {
                    ModelState.AddModelError("emailAddress", "کاربری با این ایمیل یافت نشد.");
                }
            }
            return View(forgot);
        }
        public ActionResult RecoveryPassword(string id)
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RecoveryPassword(string id, RecoveryPasswordViewModel recovery)
        {
            if (ModelState.IsValid)
            {

                var user = db.ShopUser.SingleOrDefault(s => s.ActiveCode == id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.password = FormsAuthentication.HashPasswordForStoringInConfigFile(recovery.password, "MD5");
                user.ActiveCode = Guid.NewGuid().ToString();
                db.SaveChanges();
                return Redirect("/Account/RecoveryPassword/" + id + "?recovery=true");
            }
            return View(recovery);
        }

    }
}