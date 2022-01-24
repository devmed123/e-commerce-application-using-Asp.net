using myauth.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class Proprietaire
    {
        [Key]
        public int IdPropritaire { get; set; }
       

        public string Id { get; set; }
        [ForeignKey(nameof(Id))]
        public ApplicationUser User { get; set; }

        public ICollection<Produit> Produits { get; set; }
        public ICollection<PropActivite> PropActivites { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsNoir { get; set; }
        public bool Issociete { get; set; }

    }
}