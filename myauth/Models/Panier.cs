using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class Panier
    {
        [Key]
        public int IdPanier { get; set; }
        public ICollection<Commande> Commandes { get; set; }
    }
}