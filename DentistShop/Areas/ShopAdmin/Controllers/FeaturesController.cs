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
    public class FeaturesController : Controller
    {
        private DentistShopDBEntities db = new DentistShopDBEntities();

        
        public ActionResult Index()
        {
            ViewBag.Error = "";
            return View();
        }

        public ActionResult ListFeatures()
        {
            ViewBag.Error = "";
            return PartialView(db.Features.ToList());
        }
       
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Features features = db.Features.Find(id);
            if (features == null)
            {
                return HttpNotFound();
            }
            return View(features);
        }

        
        public ActionResult Create()
        {
            ViewBag.Error = "";
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "featureTitle")] Features features)
        {
            if (ModelState.IsValid)
            {
                int id = 1;
                if (db.Features.ToList().Count != 0)
                {
                    id = db.Features.ToList().Max(m => m.featureId) + 1;
                }
                features.featureId = id;
                db.Features.Add(features);
                db.SaveChanges();
                return PartialView("ListFeatures", db.Features.ToList());
            }
            ViewBag.Error = "لطفا اطلاعات را به درستی وراد کنید.";
            return PartialView("ListFeatures", db.Features.ToList());
        }

        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Features features = db.Features.Find(id);
            if (features == null)
            {
                return HttpNotFound();
            }
            ViewBag.EditError = "";
            return View(features);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "featureId,featureTitle")] Features features)
        {
            if (ModelState.IsValid)
            {
                db.Entry(features).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("ListFeatures", db.Features.ToList());
            }
            ViewBag.EditError = "لطفا اطلاعات را به درستی وراد کنید.";
            return PartialView("ListFeatures", db.Features.ToList());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            Features features = db.Features.Find(id);
            db.Features.Remove(features);
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