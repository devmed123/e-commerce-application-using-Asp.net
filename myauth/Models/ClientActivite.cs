using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class ClientActivite
    {
       
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public int IdCommande { get; set; }
        [ForeignKey(nameof(IdCommande))]
        public Commande Commande { get; set; }
        public Client Client { get; set; }
    }
}