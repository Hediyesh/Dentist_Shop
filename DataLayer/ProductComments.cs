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
    
    [MetadataType(typeof(ProductCommentsMetaData))]
    public partial class ProductComments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductComments()
        {
            this.ProductComments1 = new HashSet<ProductComments>();
        }
    
        public int commentId { get; set; }
        public int productId { get; set; }
        public int userId { get; set; }
        public System.DateTime createDate { get; set; }
        public string commentText { get; set; }
        public Nullable<int> parentId { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual ShopUser ShopUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductComments> ProductComments1 { get; set; }
        public virtual ProductComments ProductComments2 { get; set; }
    }
}
