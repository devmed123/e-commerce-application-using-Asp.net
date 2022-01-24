using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class Commande
    {
        [Key]
        public int IdCommande { get; set; }
        [Required]
        public int Quantite { get; set; }
        [Required]
        public DateTime Date { get; set; }


        public Produit Produit { get; set; }
        public Client Client { get; set; }
        public Panier Panier { get; set; }
        
    }
}