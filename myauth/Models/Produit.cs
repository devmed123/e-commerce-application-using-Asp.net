using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class Produit
    {
        [Key]
        public int IdProduit { get; set; }
        [Required]
        public Decimal Prix { get; set; }
        [Required]
        public DateTime DateAddition { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Titre { get; set; }
        [Required]
        public int Quantite { get; set; }
        public DateTime DateLastModification { get; set; }
        [Required]
        public Boolean IsVerified { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
        public ICollection<Commande> Commandes { get; set; }
        public Proprietaire Proprietaire { get; set; }
        public ICollection<PropActivite> PropActivites { get; set; }
        public ICollection<ImageProduit> Images { get; set; }
        public ICollection<Commentaire> Commentaires { get; set; }


    }
}