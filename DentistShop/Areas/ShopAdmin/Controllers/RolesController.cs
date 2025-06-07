using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace DentistShop.Areas.ShopAdmin.Controllers
{
    public class RolesController : Controller
    {
        private DentistShopDBEntities db = new DentistShopDBEntities();

       
        public ActionResult Index()
        {
            return View(db.Roles.ToList());
        }
        
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string RoleName, string RoleTitle)
        {
            Roles roles = new Roles();
            roles.RoleName = RoleName;
            roles.RoleTitle = RoleTitle;
            var id = 1;
            if (db.Roles.ToList().Count != 0)
            {
                id = db.Roles.ToList().Max(m => m.RoleId) + 1;
            }
            roles.RoleId = id;
            if (ModelState.IsValid)
            {
                if (db.Roles.Any(a => a.RoleName == roles.RoleName))
                {
                    return Json("عنوان نقش تکراری می باشد.");
                }
                if (db.Roles.Any(a => a.RoleTitle == roles.RoleTitle))
                {
                    return Json("عنوان سیستمی نقش تکراری می باشد.");
                }
                db.Roles.Add(roles);
                db.SaveChanges();
                return Json(roles);
            }

            return Json(false);
        }

        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int RoleId, string RoleName, string RoleTitle)
        {
            var role = db.Roles.Find(RoleId);
            if (ModelState.IsValid)
            {
                if (db.Roles.Any(a => a.RoleName == RoleName && a.RoleId != RoleId))
                {
                    return Json("عنوان نقش تکراری می باشد.");
                }
                if (db.Roles.Any(a => a.RoleTitle == RoleTitle && a.RoleId != RoleId))
                {
                    return Json("عنوان سیستمی نقش تکراری می باشد.");
                }
                role.RoleName = RoleName;
                role.RoleTitle = RoleTitle;
                db.SaveChanges();
                return Json(role);
            }
            return Json(false);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            Roles roles = db.Roles.Find(id);
            db.Roles.Remove(roles);
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