using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class ProductCategories
    {
        [Key]
        [Column(Order=1)]
        public int ProductId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Category_id { get; set; }
        public virtual Produit Produit { get; set; }
        public virtual Categorie Categorie { get; set; }
    }
}