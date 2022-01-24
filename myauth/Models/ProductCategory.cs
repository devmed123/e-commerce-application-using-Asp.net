using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class ProductCategory
    {
        [Key, Column(Order = 1)]
        public int IdProduit { get; set; }
        [Key, Column(Order = 2)]
        public int IdCategorie { get; set; }
        public Produit produit { get; set; }
        public Categorie Categorie { get; set; }
    }
}