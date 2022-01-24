using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class Commentaire
    {
        [Key]
        public int IdCommentaire { get; set; }
        [Required]
        public string Message { get; set; }


        public Produit Produit { get; set; }
        public Client Client { get; set; }
    }
}