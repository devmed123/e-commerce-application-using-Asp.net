using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class ImageProduit
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public byte[] Image { get; set; }
        public Produit Produit { get; set; }

    }
}