using myauth.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace myauth.Models
{
    public class Admin
    {
        [Key]
        public int IdAdmin { get; set; }
        public String Id { get; set; }
        [ForeignKey(nameof(Id))]
        public ApplicationUser User { get; set; }
        public DateTime Last_connection { get; set; }


    }
}