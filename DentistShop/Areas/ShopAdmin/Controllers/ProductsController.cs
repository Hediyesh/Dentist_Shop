using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using System.IO;

namespace DentistShop.Areas.ShopAdmin.Controllers
{
    public class ProductsController : Controller
    {
        private DentistShopDBEntities db = new DentistShopDBEntities();

       
        public ActionResult Index()
        {
            return View(db.Product.ToList());
        }
        
        public ActionResult Create()
        {
            ViewBag.Groups = db.ProductGroup.ToList();
            ViewBag.SelectedGroupError = false;
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "productName,Price,discount,productcount")] Product product, List<int> selectedGroups, HttpPostedFileBase imageProduct)
        {
            ViewBag.Groups = db.ProductGroup.ToList();
            if (ModelState.IsValid)
            {
                if (selectedGroups == null)
                {
                    ViewBag.SelectedGroupError = true;
                    return View(product);
                }
                int id = 1;
                if (db.Product.ToList().Count != 0)
                {
                    id = db.Product.ToList().Max(m => m.productId) + 1;
                }
                product.imageName = "NoImage.jpg";
                if (imageProduct != null && imageProduct.IsImage())
                {
                    product.imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageProduct.FileName);
                    imageProduct.SaveAs(Server.MapPath("/Pictures/ProductImages/" + product.imageName));
                }
                product.productId = id;
                if (product.discount == null)
                {
                    product.discount = 0;
                }
                if (product.productcount == null)
                {
                    product.productcount = 0;
                }
                if (product.likesCount == null)
                {
                    product.likesCount = 0;
                }
                if (product.viewCount == null)
                {
                    product.viewCount = 0;
                }
                if (product.rate == null)
                {
                    product.rate = 0;
                }
                product.productPriceWithDiscont = (int)(product.Price - ((double)((double)product.discount / 100) * product.Price));
                product.createDate = DateTime.Now;
                db.Product.Add(product);
                int selectedid = 1;
                if (db.ProductSelectedGroups.ToList().Count != 0)
                {
                    selectedid = db.ProductSelectedGroups.ToList().Max(m => m.id) + 1;
                }
                foreach (var item in selectedGroups)
                {
                    db.ProductSelectedGroups.Add(new ProductSelectedGroups()
                    {
                        id = selectedid,
                        groupId = item,
                        productId = product.productId
                    });
                    selectedid++;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public ActionResult Edit(int? id)
        {
            ViewBag.SelectedGroupError = false;
            ViewBag.Groups = db.ProductGroup.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.SelectedGroups = product.ProductSelectedGroups.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "productId,productName,Price,imageName,createDate,discount,productcount")] Product product, List<int> selectedGroups, HttpPostedFileBase imageProduct)
        {
            ViewBag.Groups = db.ProductGroup.ToList();
            ViewBag.SelectedGroups = product.ProductSelectedGroups.ToList();
            if (ModelState.IsValid)
            {
                if (selectedGroups == null)
                {
                    ViewBag.SelectedGroupError = true;
                    return View(product);
                }
                if (imageProduct != null && imageProduct.IsImage())
                {
                    if (product.imageName != "NoImage.jpg")
                    {
                        System.IO.File.Delete(Server.MapPath("/Pictures/ProductImages/" + product.imageName));
                    }
                    product.imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageProduct.FileName);
                    imageProduct.SaveAs(Server.MapPath("/Pictures/ProductImages/" + product.imageName));
                }
                product.productPriceWithDiscont = (int)(product.Price - ((double)((double)product.discount / 100) * product.Price));
                db.Entry(product).State = EntityState.Modified;
                db.ProductSelectedGroups.Where(w => w.productId == product.productId).ToList().ForEach(s => db.ProductSelectedGroups.Remove(s));
                int selectedid = 1;
                if (db.ProductSelectedGroups.ToList().Count != 0)
                {
                    selectedid = db.ProductSelectedGroups.ToList().Max(m => m.id) + 1;
                }
                foreach (var item in selectedGroups)
                {
                    db.ProductSelectedGroups.Add(new ProductSelectedGroups()
                    {
                        id = selectedid,
                        groupId = item,
                        productId = product.productId
                    });
                    selectedid++;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SelectedGroups = selectedGroups;
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            if (db.ProductSelectedGroups.Any(a => a.productId == id))
            {
                foreach (var item in db.ProductSelectedGroups.Where(a => a.productId == id).ToList())
                {
                    db.ProductSelectedGroups.Remove(item);
                }
            }
            if (db.Likes.Any(a => a.productId == id))
            {
                foreach (var item in db.Likes.Where(a => a.productId == id).ToList())
                {
                    db.Likes.Remove(item);
                }
            }
            if (db.ProductComments.Any(a => a.productId == id))
            {
                foreach (var item in db.ProductComments.Where(a => a.productId == id).ToList())
                {
                    db.ProductComments.Remove(item);
                }
            }
            if (db.ProductGalleries.Any(a => a.productId == id))
            {
                foreach (var item in db.ProductGalleries.Where(a => a.productId == id).ToList())
                {
                    System.IO.File.Delete(Server.MapPath("/Pictures/ProductImages/" + item.imageName));
                    db.ProductGalleries.Remove(item);
                }
            }
            if (db.ProductFeatures.Any(a => a.productId == id))
            {
                foreach (var item in db.ProductFeatures.Where(a => a.productId == id).ToList())
                {
                    db.ProductFeatures.Remove(item);
                }
            }
            if (db.Likes.Any(a => a.productId == id))
            {
                foreach (var item in db.Likes.Where(a => a.productId == id).ToList())
                {
                    db.Likes.Remove(item);
                }
            }
            if (db.Rates.Any(a => a.productId == id))
            {
                foreach (var item in db.Rates.Where(a => a.productId == id).ToList())
                {
                    db.Rates.Remove(item);
                }
            }
            System.IO.File.Delete(Server.MapPath("/Pictures/ProductImages/" + product.imageName));
            db.Product.Remove(product);
            db.SaveChanges();
            return Json(id);
        }

        #region Gallery
        public ActionResult Gallery(int id)
        {
            ViewBag.Galleries = db.ProductGalleries.Where(w => w.productId == id).ToList();
            return View(new ProductGalleries() { productId = id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Gallery(ProductGalleries productGallery, HttpPostedFileBase imgUp)
        {
            if (ModelState.IsValid)
            {
                int imgid = 1;
                if (db.ProductGalleries.ToList().Count != 0)
                {
                    imgid = db.ProductGalleries.ToList().Max(m => m.galleryId) + 1;
                }
                if (imgUp != null && imgUp.IsImage())
                {
                    productGallery.imageName = Guid.NewGuid().ToString() +
                        Path.GetExtension(imgUp.FileName);
                    imgUp.SaveAs(Server.MapPath("/Pictures/ProductImages/" + productGallery.imageName));
                    productGallery.galleryId = imgid;
                    db.ProductGalleries.Add(productGallery);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Gallery", new { id = productGallery.productId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteGallery(int id)
        {
            var gallery = db.ProductGalleries.Find(id);
            System.IO.File.Delete(Server.MapPath("/Pictures/ProductImages/" + gallery.imageName));
            db.ProductGalleries.Remove(gallery);
            db.SaveChanges();
            return Json(id);
        }

        #endregion

        #region Features
        public ActionResult ProductFeatures(int id)
        {
            ViewBag.Features = db.ProductFeatures.Where(w => w.productId == id).ToList();
            ViewBag.featureId = new SelectList(db.Features, "featureId", "featureTitle");
            return View(new ProductFeatures() { productId = id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductFeatures(ProductFeatures features)
        {
            if (ModelState.IsValid)
            {
                int fid = 1;
                if (db.ProductFeatures.ToList().Count != 0)
                {
                    fid = db.ProductFeatures.ToList().Max(m => m.Id) + 1;
                }
                features.Id = fid;
                db.ProductFeatures.Add(features);
                db.SaveChanges();
            }
            return RedirectToAction("ProductFeatures", new { id = features.productId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteFeature(int id)
        {
            db.ProductFeatures.Remove(db.ProductFeatures.Find(id));
            db.SaveChanges();
            return Json(id);
        }
        #endregion
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