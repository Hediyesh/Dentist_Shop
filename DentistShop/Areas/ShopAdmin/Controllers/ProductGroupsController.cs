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
    public class ProductGroupsController : Controller
    {
        private DentistShopDBEntities db = new DentistShopDBEntities();
       
        public ActionResult Index()
        {
            ViewBag.Error = "";
            return View();
        }
        public ActionResult ListGroups()
        {
            var productGroup = db.ProductGroup.Where(w => w.parentId == null);
            return PartialView(productGroup.ToList());
        }
        
        public ActionResult Create(int? id)
        {
            ViewBag.Error = "";
            return View(new ProductGroup() { parentId = id });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "groupId,groupName,parentId")] ProductGroup productGroup)
        {
            if (ModelState.IsValid)
            {
                var id = 1;
                if (db.ProductGroup.ToList().Count != 0)
                {
                    id = db.ProductGroup.ToList().Max(m => m.groupId) + 1;
                }
                productGroup.groupId = id;
                db.ProductGroup.Add(productGroup);
                db.SaveChanges();
                return PartialView("ListGroups", db.ProductGroup.Where(w => w.parentId == null));
            }
            ViewBag.Error = "لطفا اطلاعات را به درستی وراد کنید.";
            return PartialView("ListGroups", db.ProductGroup.Where(w => w.parentId == null));
        }

        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGroup productGroup = db.ProductGroup.Find(id);
            if (productGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.EditError = "";
            return View(productGroup);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "groupId,groupName,parentId")] ProductGroup productGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productGroup).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("ListGroups", db.ProductGroup.Where(w => w.parentId == null));
            }
            ViewBag.EditError = "لطفا اطلاعات را به درستی وراد کنید.";
            return PartialView("ListGroups", db.ProductGroup.Where(w => w.parentId == null));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            ProductGroup productGroup = db.ProductGroup.Find(id);
            db.ProductGroup.Remove(productGroup);
            if (db.ProductGroup.Any(a => a.parentId == id))
            {
                foreach (var item in db.ProductGroup.Where(w => w.parentId == id))
                {
                    db.ProductGroup.Remove(item);
                }
            }
            db.SaveChanges();
            return Json(id);
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