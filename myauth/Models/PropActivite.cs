using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class PropActivite
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public Produit Produit { get; set; }
        public Proprietaire Proprietaire { get; set; }

    }
}