using myauth.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class Client
    {
        [Key]
        public int IdClient { get; set; }

        public string Id { get; set; }
        [ForeignKey(nameof(Id))]
        public ApplicationUser User { get; set; }

        public ICollection<Categorie> Categories { get; set; }
        public ICollection<Commande> Commandes { get; set; }
        public ICollection<Commentaire> Commentaires { get; set; }
        public ICollection<ClientActivite> ClientActivites { get; set; }


    }
}