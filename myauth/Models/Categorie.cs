using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class Categorie
    {
        [Key]
        public int IdCategorie { get; set; }
        [Required]
        public string Nom { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
    }
     
    //TODO:Hajar , Bouzit : Create here the enumeration of the categories that u will use in filters (Create a default one that will get all the products)
    // this enum will be attribute in the prop "Nom" 
}