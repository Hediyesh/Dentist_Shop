using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.Ajax.Utilities;

namespace DentistShop.Controllers
{
    public class ProductController : Controller
    {
        private DentistShopDBEntities db = new DentistShopDBEntities();

        [Route("ShowProduct/{id}")]
        public ActionResult ShowProduct(int id)
        {
            var product = db.Product.SingleOrDefault(s => s.productId == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.productId = product.productId;
            var showProduct = product;
            if (showProduct.likesCount == null)
            {
                showProduct.likesCount = 0;
            }
            if (showProduct.rate == null)
            {
                showProduct.rate = 0;
            }
            if (showProduct.viewCount == null)
            {
                showProduct.viewCount = 0;
            }
            ViewBag.productFeatures = db.ProductFeatures.Where(w=>w.productId==product.productId).DistinctBy(d => d.featureId).Select(s => new ShowProductFeaturesViewModel()
            {
                featureTitle = s.Features.featureTitle,
                values = db.ProductFeatures.Where(w => w.featureId == s.featureId && w.productId==product.productId).Select(t => t.value).ToList()
            }).ToList();
            if (db.ProductFeatures.Where(w => w.Features.featureTitle == "جنس" && w.productId == id).ToList().Count != 0)
            {
                ViewBag.Jens = db.ProductFeatures.Where(w => w.Features.featureTitle == "جنس" && w.productId == id).ToList();
            }
            if (db.ProductFeatures.Where(w => w.Features.featureTitle == "کشور سازنده" && w.productId == id).ToList().Count != 0)
            {
                ViewBag.Keshvar = db.ProductFeatures.Where(w => w.Features.featureTitle == "کشور سازنده" && w.productId == id).ToList();
            }
            if (db.ProductFeatures.Where(w => w.Features.featureTitle == "برند" && w.productId == id).ToList().Count != 0)
            {
                ViewBag.brand = db.ProductFeatures.Where(w => w.Features.featureTitle == "برند" && w.productId == id).ToList();
            }
            return View(showProduct);
        }

        [Route("AllPicsOfProduct/{id}")]
        public ActionResult AllPicsOfProduct(int id)
        {
            ViewBag.productname = db.Product.Find(id).productName;
            var gallery = db.ProductGalleries.Where(w => w.productId == id).ToList();
            return View(gallery);
        }
        public ActionResult ShowGroups()
        {
            return PartialView(db.ProductGroup.ToList());
        }
        public ActionResult NewestProducts()
        {
            var products = db.Product.OrderByDescending(p => p.createDate).Take(2).ToList();
            return PartialView(products);
        }
        public ActionResult ProductsCheapest()
        {
            var products = db.Product.OrderBy(p => p.productPriceWithDiscont).Take(2).ToList();
            return PartialView(products);
        }
        [Route("Archive")]
        public ActionResult Archive(int pageId = 1, int change = 0, string title = "", int minValue = 0, int maxValue = 0, List<int> selectedGroups = null)
        {
            ViewBag.ProductTitle = title;
            ViewBag.PageId = pageId;
            ViewBag.selectedgroups = selectedGroups;
            ViewBag.ProductGroups = db.ProductGroup.ToList();
            List<Product> products = new List<Product>();
            if (selectedGroups != null && selectedGroups.Any())
            {
                foreach (var item in selectedGroups)
                {
                    products.AddRange(db.ProductSelectedGroups.Where(w => w.groupId == item).Select(s => s.Product).ToList());
                }
                products = products.Distinct().ToList();
            }
            else
            {
                products.AddRange(db.Product.ToList());
            }
            if (title != "")
            {
                products = products.Where(w => w.productName.Contains(title) || title.Contains(w.productName)).ToList();
            }
            if (minValue <= 0)
            {
                ViewBag.minvalue = db.Product.Min(m => m.Price);
            }
            else
            {
                ViewBag.minvalue = minValue;
                products = products.Where(w => w.Price >= minValue).ToList();
            }
            if (maxValue <= 0)
            {
                ViewBag.maxvalue = db.Product.Max(m => m.Price);
            }
            else
            {
                ViewBag.maxvalue = maxValue;
                products = products.Where(w => w.Price <= maxValue).ToList();
            }
            //page bandi
            if (change == 1)
            {
                pageId = 1;
            }
            int take = 6;
            int skip = (pageId - 1) * take;
            ViewBag.PageCount = products.Count / take;
            return View(products.Distinct().OrderByDescending(o => o.productId).Skip(skip).Take(take).ToList());
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult AddToShopCart(int count, int pageProductId)
        {
            var user = db.ShopUser.Where(w => w.userName == User.Identity.Name).Single();
            var product = db.Product.Find(pageProductId);
            ShoppingCart cart = new ShoppingCart();
            int id = 1;
            if (db.ShoppingCart.Count() > 0)
            {
                id = db.ShoppingCart.Max(m => m.cartid) + 1;
            }
            cart.productId = pageProductId;
            cart.userId = user.userId;
            cart.cartid = id;
            cart.count = count;
            cart.sabtDate = DateTime.Now;
            cart.isfinal = false;
            cart.wholeprice = count * product.productPriceWithDiscont;
            db.ShoppingCart.Add(cart);
            db.SaveChanges();
            return Json(true);
        }
        [Route("ShoppingCart/{id}")]
        public ActionResult ShoppingCart(string id)
        {
            var shopcarts = db.ShoppingCart.Where(w => w.ShopUser.userName == id && w.isfinal == false).ToList();
            ViewBag.username = "";
            if (db.ShopUser.SingleOrDefault(s => s.userName == id)!=null)
            {
                ViewBag.username = db.ShopUser.Single(s => s.userName == id).userName;
            }
            return View(shopcarts);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult RemoveFromUserShopCart(int id)
        {
            db.ShoppingCart.Remove(db.ShoppingCart.Find(id));
            db.SaveChanges();
            return Json(true);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Pay(List<int> checks)
        {
            int userid = db.ShopUser.Single(si => si.userName == User.Identity.Name).userId;
            var lish = db.ShoppingCart.Where(w => w.userId == userid && w.isfinal==false).ToList();
            var orderli = new List<ShoppingCart>();
            foreach (var item in checks)
            {
                if (lish.Any(w=>w.cartid==item))
                {
                    orderli.Add(lish.Single(si => si.cartid == item));
                }
            }
            var order = new Orders();
            int idorder = 1;
            if (db.Orders.ToList().Count==0)
            {
                order.orderid = idorder;
            }
            else
            {
                order.orderid = db.Orders.Max(m => m.orderid) + 1;
            }
            List<OrderListShopCarts> li = new List<OrderListShopCarts>();
            int osId = 1;
            if (db.OrderListShopCarts.Count()!=0)
            {
                osId = db.OrderListShopCarts.Max(m => m.id) + 1;
            }
            int sum = 0;
            foreach (var item in orderli)
            {
                OrderListShopCarts os = new OrderListShopCarts();
                os.id = osId;
                osId++;
                os.isfinal = false;
                os.orderid = order.orderid;
                os.userId = userid;
                os.cartid = item.cartid;
                db.OrderListShopCarts.Add(os);
                sum += item.wholeprice;
            }
            order.userId = userid;
            order.RefID = 0;
            order.wholeprice = sum;
            order.sabtDate = DateTime.Now;
            order.isfinal = false;
            db.Orders.Add(order);
            db.SaveChanges();


            //online payment
            System.Net.ServicePointManager.Expect100Continue = false;
            ZarinPal.PaymentGatewayImplementationServicePortTypeClient zp = new ZarinPal.PaymentGatewayImplementationServicePortTypeClient();
            string Authority;
            int Status = zp.PaymentRequest("YOUR-ZARINPAL-MERCHANT-CODE", order.wholeprice, "پرداخت آنلاین", "hediye.shariaty@gmail.com", "09031824882", "https://localhost:44349/Product/Verify/" + order.orderid, out Authority);
            string s = "";
            if (Status == 100)
            {
                // Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority);
                s = "https://sandbox.zarinpal.com/pg/StartPay/" + Authority;
                return Json(s);
            }
            else
            {
                s = "Error : " + Status;
            }
            return Json(s);

        }
        public ActionResult Verify(int id)
        {
            var order = db.Orders.Find(id);
            var osli = db.OrderListShopCarts.Where(w => w.orderid == id).ToList();
            var shopcartli = new List<ShoppingCart>();
            foreach (var item in db.ShoppingCart)
            {
                if (osli.Any(a=>a.cartid==item.cartid))
                {
                    shopcartli.Add(item);
                }
            }
            if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null && Request.QueryString["Authority"] != "" && Request.QueryString["Authority"] != null)
            {
                if (Request.QueryString["Status"].ToString().Equals("OK"))
                {
                    int Amount = order.wholeprice;
                    long RefID;
                    System.Net.ServicePointManager.Expect100Continue = false;
                    ZarinPal.PaymentGatewayImplementationServicePortTypeClient zp = new ZarinPal.PaymentGatewayImplementationServicePortTypeClient();

                    int Status = zp.PaymentVerification("YOUR-ZARINPAL-MERCHANT-CODE", Request.QueryString["Authority"].ToString(), Amount, out RefID);

                    if (Status == 100)
                    {
                        order.isfinal = true;
                        order.RefID = RefID;
                        foreach (var item in osli)
                        {
                            item.isfinal = true;
                        }
                        foreach (var item in shopcartli)
                        {
                            item.isfinal = true;
                        }
                        db.SaveChanges();
                        ViewBag.IsSuccess = true;
                        ViewBag.RefId = RefID;
                    }
                    else
                    {
                        ViewBag.Status = Status;
                    }

                }
                else
                {
                    Response.Write("Error! Authority: " + Request.QueryString["Authority"].ToString() + " Status: " + Request.QueryString["Status"].ToString());
                }
            }
            else
            {
                Response.Write("Invalid Input");
            }
            return View();
        }
        #region Comment
        public ActionResult ShowComments(int id)
        {
            return PartialView(db.ProductComments.Where(a => a.productId == id).ToList());
        }
        public ActionResult CreateComment(int id, string username)
        {
            var newCom = new ProductComments() { productId = id, userId = db.ShopUser.Single(s => s.userName == username).userId };
            return PartialView(newCom);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateNewComment(string commentId, string productId, string parentId, string commentText)
        {
            var pId = Convert.ToInt32(productId);
            var comments = new ProductComments();
            if (commentText.Trim().Length != 0)
            {
                if (commentId == "" || commentId == "0")
                {
                    int comid = 1;
                    if (db.ProductComments.ToList().Count != 0)
                    {
                        comid = db.ProductComments.ToList().Max(m => m.commentId) + 1;
                    }
                    comments.createDate = DateTime.Now;
                    comments.commentId = comid;
                    comments.productId = Convert.ToInt32(productId);
                    comments.userId = db.ShopUser.Single(s => s.userName == User.Identity.Name).userId;
                    comments.commentText = commentText;
                    if (!(parentId == "" || parentId == "0"))
                    {
                        comments.parentId = Convert.ToInt32(parentId);
                    }
                    db.ProductComments.Add(comments);

                }
                else
                {
                    var com = db.ProductComments.Find(Convert.ToInt32(commentId));
                    com.commentText = commentText;
                }
                db.SaveChanges();
                return PartialView("ShowComments", db.ProductComments.Where(a => a.productId == pId).ToList());

            }
            else
            {
                return PartialView("ShowComments", db.ProductComments.Where(a => a.productId == pId).ToList());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditComment(int id)
        {
            return Json(db.ProductComments.Single(s => s.commentId == id).commentText);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int id)
        {
            var comment = db.ProductComments.Find(id);
            var productid = comment.productId;
            if (db.ProductComments.Any(a => a.parentId == id))
            {
                foreach (var item in db.ProductComments.Where(a => a.parentId == id).ToList())
                {
                    item.parentId = null;
                }
            }
            db.ProductComments.Remove(comment);
            db.SaveChanges();
            return PartialView("ShowComments", db.ProductComments.Where(a => a.productId == productid).ToList());
        }
        #endregion

        #region Like
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LikeProduct(int userIsAuthenticated, int pageProductId)
        {
            if (userIsAuthenticated == 0)
            {
                return Json("لطفا ابتدا وارد سایت شوید.");
            }
            var user = db.ShopUser.Where(w => w.userName == User.Identity.Name).Single();
            var likeproduct = db.Likes.Where(w => w.userId == user.userId && w.productId == pageProductId).SingleOrDefault();
            if (likeproduct == null)
            {
                var product = db.Product.Find(pageProductId);
                Likes like = new Likes();
                if (db.Likes.ToList().Count == 0)
                {
                    like.likeId = 1;
                }
                else
                {
                    var max = db.Likes.Max(o => o.likeId);
                    like.likeId = max + 1;
                }
                like.userId = user.userId;
                like.productId = product.productId;
                db.Likes.Add(like);
                product.likesCount++;
                db.SaveChanges();
                return Json(true);
            }
            return Json(false);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UnLikeProduct(int userIsAuthenticated, int pageProductId)
        {
            if (userIsAuthenticated == 0)
            {
                return Json("لطفا ابتدا وارد سایت شوید.");
            }
            var user = db.ShopUser.Single(s => s.userName == User.Identity.Name);
            var product = db.Product.Find(pageProductId);
            var likeproduct = db.Likes.Where(w => w.userId == user.userId && w.productId == pageProductId).SingleOrDefault();
            if (likeproduct != null)
            {
                db.Likes.Remove(likeproduct);
                product.likesCount--;
                db.SaveChanges();
                return Json(true);
            }
            return Json(false);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult IsLiked(int pageProductId)
        {
            var like = db.Likes.Where(w => w.ShopUser.userName == User.Identity.Name && w.productId == pageProductId).SingleOrDefault();
            if (like != null)
            {
                return Json(true);
            }
            return Json(false);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AreLikedProducts()
        {
            var like = db.Likes.Where(w => w.ShopUser.userName == User.Identity.Name).Select(s => s.productId).ToList();
            if (like.Count > 0)
            {
                return Json(like);
            }
            return Json(false);
        }
        #endregion

        #region Rate
        public ActionResult RatingPartial()
        {
            return PartialView();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SetRate(int pageProductId, string value)
        {
            double count = 0;
            double sum = 0;
            var dvalue = double.Parse(value, CultureInfo.InvariantCulture);
            var product = db.Product.Find(pageProductId);
            var userid = db.ShopUser.Where(w => w.userName == User.Identity.Name).Single().userId;
            if (db.Rates.Any(a => a.userId == userid && a.productId == product.productId))
            {
                var rate = db.Rates.Where(a => a.userId == userid && a.productId == product.productId).Single();
                rate.rate = dvalue;
            }
            else
            {
                Rates rate = new Rates();
                int id = 1;
                if (db.Rates.Count() > 0)
                {
                    id = db.Rates.Max(m => m.rateId) + 1;
                }
                rate.rateId = id;
                rate.productId = pageProductId;
                rate.userId = userid;
                rate.rate = dvalue;
                db.Rates.Add(rate);
                count++;
                sum += dvalue;
            }
            var rateli = db.Rates.Where(w => w.productId == pageProductId).ToList();
            if (rateli.Count > 0)
            {
                foreach (var item in rateli)
                {
                    count++;
                    sum += item.rate;
                }
            }
            product.rate = Math.Round(sum / count, 2);
            db.SaveChanges();
            return Json(product.rate.ToString().Replace('.', '/'));
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult IsSetRateByUser(int pageProductId)
        {
            var product = db.Product.Find(pageProductId);
            var userid = db.ShopUser.Where(w => w.userName == User.Identity.Name).Single().userId;
            if (db.Rates.Any(a => a.userId == userid && a.productId == product.productId))
            {
                return Json(db.Rates.Where(a => a.userId == userid && a.productId == product.productId).Single().rate / 0.5);
            }
            return Json(false);
        }
        #endregion


    }
}