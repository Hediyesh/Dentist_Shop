using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DentistShop.Controllers
{
    public class ManageEmailsController : Controller
    {

        private DentistShopDBEntities db = new DentistShopDBEntities();
        public ActionResult RecoveryPassword()
        {
            return PartialView();
        }
    }
}