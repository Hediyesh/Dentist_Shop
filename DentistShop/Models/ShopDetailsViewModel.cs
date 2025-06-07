using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DentistShop
{
    public class ShopDetailsViewModel
    {
        [Key]
        public int orlishopid { get; set; }
        public int userId { get; set; }
        public int cartid { get; set; }
        public int productId { get; set; }
        public string productname { get; set; }
        public int count { get; set; }
        public int wholeprice { get; set; }

    }
}