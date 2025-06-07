using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DentistShop.Areas.UserPanel.Controllers
{
    public class PanelController : Controller
    {
        private DentistShopDBEntities db = new DentistShopDBEntities();
        // GET: UserPanel/Home
        public ActionResult PanelPage(string id)
        {
            var user = new ShopUser();
            foreach (var item in db.ShopUser.ToList())
            {
                if (FormsAuthentication.HashPasswordForStoringInConfigFile(item.userName, "MD5") == id)
                {
                    user = item;
                }
            }
            if (user.userId == 0)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        public ActionResult EditProfile(string id)
        {
            var shopUser = new ShopUser();
            foreach (var item in db.ShopUser.ToList())
            {
                if (FormsAuthentication.HashPasswordForStoringInConfigFile(item.userName, "MD5") == id)
                {
                    shopUser = item;
                }
            }
            if (shopUser == null)
            {
                return HttpNotFound();
            }
            return View(shopUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "userId,userName,firstName,lastName,emailAddress,password,phone,Address")] ShopUser shopUser)
        {
            var user = db.ShopUser.Find(shopUser.userId);
            var user1 = db.ShopUser.Where(a => a.userId != shopUser.userId && a.userName == shopUser.userName).SingleOrDefault();
            var user2 = db.ShopUser.Where(a => a.userId != shopUser.userId && a.emailAddress == shopUser.emailAddress.Trim().ToLower()).SingleOrDefault();
            if (ModelState.IsValid)
            {
                if (user1 != null)
                {
                    ModelState.AddModelError("userName", "نام کاربری وارد شده تکراری می باشد.");
                    return View(shopUser);
                }
                else if (user2 != null)
                {
                    ModelState.AddModelError("emailAddress", "ایمیل وارد شده تکراری می باشد.");
                    return View(shopUser);
                }
                else
                {
                    user.userName = shopUser.userName;
                    user.firstName = shopUser.firstName;
                    user.lastName = shopUser.lastName;
                    user.emailAddress = shopUser.emailAddress;
                    user.phone = shopUser.phone;
                    user.Address = shopUser.Address;
                    db.SaveChanges();
                    return RedirectToAction("PanelPage/" + FormsAuthentication.HashPasswordForStoringInConfigFile(user.userName, "MD5"));
                }
            }
            return View(shopUser);
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel changePassword)
        {
            if (ModelState.IsValid)
            {
                var user = db.ShopUser.Single(s => s.userName == User.Identity.Name);
                var ramz = FormsAuthentication.HashPasswordForStoringInConfigFile(changePassword.oldPassword, "MD5");
                if (ramz != user.password)
                {
                    ModelState.AddModelError("oldPassword", "رمز عبور وارد شده نادرست می باشد.");
                }
                else
                {
                    user.password = FormsAuthentication.HashPasswordForStoringInConfigFile(changePassword.password, "MD5");
                    db.SaveChanges();
                    ViewBag.success = true;
                }

            }
            return View();
        }
        public ActionResult ShopDetails(int id)
        {
            List<ShopDetailsViewModel> li = new List<ShopDetailsViewModel>();
            var orderli = db.OrderListShopCarts.Where(w => w.orderid == id).ToList();
            foreach (var item in orderli)
            {
                ShopDetailsViewModel sd = new ShopDetailsViewModel();
                sd.orlishopid = item.id;
                sd.cartid = item.cartid;
                sd.count = item.ShoppingCart.count;
                sd.productId = item.ShoppingCart.productId;
                sd.productname = item.ShoppingCart.Product.productName;
                sd.userId = item.userId;
                sd.wholeprice = item.ShoppingCart.wholeprice;
                li.Add(sd);
            }
            return View(li);
        }
    }
}