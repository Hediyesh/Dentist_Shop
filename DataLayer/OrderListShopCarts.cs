//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLayer
{
    using System.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    
    [MetadataType(typeof(OrderListShopCartsMetaData))]
    public partial class OrderListShopCarts
    {
        public int id { get; set; }
        public int orderid { get; set; }
        public int userId { get; set; }
        public int cartid { get; set; }
        public bool isfinal { get; set; }
    
        public virtual ShoppingCart ShoppingCart { get; set; }
        public virtual ShopUser ShopUser { get; set; }
        public virtual Orders Orders { get; set; }
    }
}
