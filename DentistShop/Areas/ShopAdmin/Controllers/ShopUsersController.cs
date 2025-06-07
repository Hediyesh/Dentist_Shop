using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DataLayer;

namespace DentistShop.Areas.ShopAdmin.Controllers
{
    public class ShopUsersController : Controller
    {
        private DentistShopDBEntities db = new DentistShopDBEntities();

        
        public ActionResult Index()
        {
            var shopUser = db.ShopUser.Include(s => s.Roles);
            return View(shopUser.ToList());
        }

        
        public ActionResult Create()
        {
            ViewBag.RoleIds = new SelectList(db.Roles, "RoleId", "RoleName");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userName,firstName,lastName,emailAddress,password,IsActive,phone,Address")] ShopUser shopUser, int SelestedRoleId)
        {
            shopUser.RoleId = SelestedRoleId;
            if (shopUser.RoleId == -1)
            {
                shopUser.RoleId = 1;
            }
            ViewBag.RoleIds = new SelectList(db.Roles, "RoleId", "RoleName", shopUser.RoleId);
            var userid = 1;
            if (db.ShopUser.ToList().Count != 0)
            {
                userid = db.ShopUser.ToList().Max(m => m.userId) + 1;
            }
            if (ModelState.IsValid)
            {
                if (db.ShopUser.Any(a => a.userName == shopUser.userName))
                {
                    ModelState.AddModelError("userName", "نام کاربری وارد شده تکراری می باشد.");
                    return View(shopUser);
                }
                if (db.ShopUser.Any(a => a.emailAddress == shopUser.emailAddress.Trim().ToLower()))
                {
                    ModelState.AddModelError("emailAddress", "ایمیل وارد شده تکراری می باشد.");
                    return View(shopUser);
                }
                shopUser.userId = userid;
                shopUser.RegisterDate = DateTime.Now;
                shopUser.ActiveCode = Guid.NewGuid().ToString();
                shopUser.password = FormsAuthentication.HashPasswordForStoringInConfigFile(shopUser.password, "MD5");
                db.ShopUser.Add(shopUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shopUser);
        }

        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopUser shopUser = db.ShopUser.Find(id);
            if (shopUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", shopUser.RoleId);
            return View(shopUser);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userId,userName,firstName,lastName,emailAddress,password,RoleId,ActiveCode,IsActive,RegisterDate,phone,Address")] ShopUser shopUser)
        {
            var user = db.ShopUser.Find(shopUser.userId);
            
            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", shopUser.RoleId);
            if (ModelState.IsValid)
            {
              
                if (db.ShopUser.Any(a => a.userId != shopUser.userId && a.userName == shopUser.userName))
                {
                    ModelState.AddModelError("userName", "نام کاربری وارد شده تکراری می باشد.");
                    return View(shopUser);
                }
                if (db.ShopUser.Any(a => a.userId != shopUser.userId && a.emailAddress == shopUser.emailAddress.Trim().ToLower()))
                {
                    ModelState.AddModelError("emailAddress", "ایمیل وارد شده تکراری می باشد.");
                    return View(shopUser);
                }
                user.userName = shopUser.userName;
                user.firstName = shopUser.firstName;
                user.lastName = shopUser.lastName;
                user.ActiveCode = shopUser.ActiveCode;
                user.IsActive = shopUser.IsActive;
                user.emailAddress = shopUser.emailAddress;
                user.password = shopUser.password;
                user.RoleId = shopUser.RoleId;
                user.phone = shopUser.phone;
                user.Address = shopUser.Address;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shopUser);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            ShopUser shopUser = db.ShopUser.Find(id);
            if (db.ProductComments.Any(a => a.userId == id))
            {
                foreach (var item in db.ProductComments.Where(a => a.userId == id).ToList())
                {
                    db.ProductComments.Remove(item);
                }
            }
            if (db.Likes.Any(a => a.userId == id))
            {
                foreach (var item in db.Likes.Where(a => a.userId == id).ToList())
                {
                    db.Likes.Remove(item);
                }
            }
            if (db.Likes.Any(a => a.userId == id))
            {
                foreach (var item in db.Likes.Where(a => a.userId == id).ToList())
                {
                    db.Likes.Remove(item);
                }
            }
            if (db.Rates.Any(a => a.userId == id))
            {
                foreach (var item in db.Rates.Where(a => a.userId == id).ToList())
                {
                    db.Rates.Remove(item);
                }
            }
            db.ShopUser.Remove(shopUser);
            db.SaveChanges();
            return Json(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}